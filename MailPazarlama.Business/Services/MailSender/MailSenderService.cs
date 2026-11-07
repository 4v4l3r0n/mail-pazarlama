using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using MailPazarlama.Business.Common;
using MailPazarlama.Business.Services.MailSetting;
using MailPazarlama.Business.Services.Security;
using MailPazarlama.Data.UnitOfWork;
using MailPazarlama.Entities;
using MailTemplateEntity = MailPazarlama.Entities.MailTemplate;
using SubscriberEntity = MailPazarlama.Entities.Subscriber;

namespace MailPazarlama.Business.Services.MailSender;

public class MailSenderService : IMailSenderService
{
    private readonly IMailSettingService _mailSettingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryptionService _encryptionService;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MailSenderService(
        IMailSettingService mailSettingService,
        IUnitOfWork unitOfWork,
        IEncryptionService encryptionService,
        IServiceScopeFactory serviceScopeFactory)
    {
        _mailSettingService = mailSettingService;
        _unitOfWork = unitOfWork;
        _encryptionService = encryptionService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<ServiceResult> SendBulkMailAsync(int templateId, List<int> subscriberIds, int userId)
    {
        if (subscriberIds == null || subscriberIds.Count == 0)
        {
            return ServiceResult.Fail("Lütfen gönderim yapılacak en az bir abone seçiniz.");
        }

        // 1. SMTP Ayarları Kontrolü
        var settings = await _mailSettingService.GetSettingAsync();
        if (settings == null || string.IsNullOrWhiteSpace(settings.SmtpServer) || string.IsNullOrWhiteSpace(settings.Email))
        {
            return ServiceResult.Fail("SMTP e-posta sunucu ayarları bulunamadı. Lütfen önce Ayarlar sayfasından SMTP ayarlarını kaydediniz.");
        }

        // 2. Şablon Kontrolü (Yalnızca aktif şablonlar)
        var template = await _unitOfWork.Repository<MailTemplateEntity>().GetByIdAsync(templateId);
        if (template == null)
        {
            return ServiceResult.Fail("Gönderilmek istenen e-posta şablonu bulunamadı.");
        }

        if (!template.IsActive)
        {
            return ServiceResult.Fail("Yalnızca aktif durumdaki şablonlar ile gönderim yapılabilir.");
        }

        // 3. Aboneler Kontrolü
        var subscribers = await _unitOfWork.Repository<SubscriberEntity>()
            .FindAsync(s => subscriberIds.Contains(s.Id));

        if (subscribers.Count == 0)
        {
            return ServiceResult.Fail("Seçilen aboneler sistemde bulunamadı.");
        }

        // Arka plan işi için gerekli verilerin kopyalanması
        var subscriberList = subscribers.Select(s => new { s.Id, s.Email }).ToList();
        var templateTitle = template.Title;
        var templateHtml = template.ContentHtml;
        var smtpServer = settings.SmtpServer;
        var smtpPort = settings.SmtpPort;
        var enableSsl = settings.EnableSsl;
        var senderEmail = settings.Email;
        var senderPassword = settings.Password;

        // 4. Arka Planda Asenkron Gönderim Döngüsü
        _ = Task.Run(async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var backgroundUow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            foreach (var subscriber in subscriberList)
            {
                var log = new MailSendLog
                {
                    SubscriberId = subscriber.Id,
                    TemplateId = templateId,
                    UserId = userId,
                    SentAt = DateTime.Now,
                    Status = "Başarılı",
                    ErrorMessage = null
                };

                var smtpServerNormalized = smtpServer?.Trim().ToLowerInvariant() ?? string.Empty;
                var isSimulation = smtpServerNormalized is "localhost";

                if (isSimulation)
                {
                    await Task.Delay(300);

                    var emailLower = subscriber.Email.ToLowerInvariant();
                    if (emailLower.Contains("hata") || emailLower.Contains("error"))
                    {
                        log.Status = "Başarısız";
                        log.ErrorMessage = "Simülasyon: Alıcı e-posta kutusu dolu veya ulaşılamıyor (Test Hatası).";
                    }
                    else
                    {
                        log.Status = "Başarılı";
                        log.ErrorMessage = null;
                    }
                }
                else
                {
                    try
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("Mail Pazarlama", senderEmail));
                        message.To.Add(new MailboxAddress(subscriber.Email, subscriber.Email));
                        message.Subject = templateTitle;

                        var bodyBuilder = new BodyBuilder
                        {
                            HtmlBody = templateHtml
                        };
                        message.Body = bodyBuilder.ToMessageBody();

                        using var client = new SmtpClient();
                        client.Timeout = 15000; // 15 saniye zaman aşımı kuralı

                        // SSL / TLS bağlantı seçeneği
                        var secureOptions = enableSsl
                            ? (smtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls)
                            : SecureSocketOptions.None;

                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        await client.ConnectAsync(smtpServer!, smtpPort, secureOptions);

                        if (!string.IsNullOrEmpty(senderEmail) && !string.IsNullOrEmpty(senderPassword))
                        {
                            await client.AuthenticateAsync(senderEmail, senderPassword);
                        }

                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }
                    catch (Exception ex)
                    {
                        log.Status = "Başarısız";
                        log.ErrorMessage = ex.Message;
                    }
                }

                await backgroundUow.Repository<MailSendLog>().AddAsync(log);
                await backgroundUow.SaveChangesAsync();
            }
        });

        return ServiceResult.Ok($"Seçilen {subscribers.Count} aboneye e-posta başarıyla gönderildi.");
    }
}

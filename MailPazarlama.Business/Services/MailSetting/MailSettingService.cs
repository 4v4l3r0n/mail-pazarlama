using MailPazarlama.Business.Common;
using MailPazarlama.Business.DTOs.MailSetting;
using MailPazarlama.Business.Services.Security;
using MailPazarlama.Data.UnitOfWork;
using MailSettingEntity = MailPazarlama.Entities.MailSetting;

namespace MailPazarlama.Business.Services.MailSetting;

public class MailSettingService : IMailSettingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryptionService _encryptionService;

    public MailSettingService(IUnitOfWork unitOfWork, IEncryptionService encryptionService)
    {
        _unitOfWork = unitOfWork;
        _encryptionService = encryptionService;
    }

    public async Task<MailSettingDto?> GetSettingAsync()
    {
        var settings = await _unitOfWork.Repository<MailSettingEntity>().GetAllAsync();
        var setting = settings.FirstOrDefault();

        if (setting == null)
        {
            return null;
        }

        var password = string.Empty;
        if (!string.IsNullOrEmpty(setting.EncryptedPassword))
        {
            try
            {
                password = _encryptionService.Decrypt(setting.EncryptedPassword);
            }
            catch
            {
                password = string.Empty;
            }
        }

        return new MailSettingDto
        {
            Id = setting.Id,
            SmtpServer = setting.SmtpServer,
            SmtpPort = setting.SmtpPort,
            EnableSsl = setting.EnableSsl,
            Email = setting.Email,
            Password = password,
            UpdatedAt = setting.UpdatedAt
        };
    }

    public async Task<ServiceResult> SaveOrUpdateSettingAsync(MailSettingDto dto)
    {
        var settings = await _unitOfWork.Repository<MailSettingEntity>().GetAllAsync();
        var setting = settings.FirstOrDefault();

        if (setting == null)
        {
            setting = new MailSettingEntity
            {
                SmtpServer = dto.SmtpServer.Trim(),
                SmtpPort = dto.SmtpPort,
                EnableSsl = dto.EnableSsl,
                Email = dto.Email.Trim(),
                EncryptedPassword = !string.IsNullOrEmpty(dto.Password)
                    ? _encryptionService.Encrypt(dto.Password)
                    : string.Empty,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<MailSettingEntity>().AddAsync(setting);
        }
        else
        {
            setting.SmtpServer = dto.SmtpServer.Trim();
            setting.SmtpPort = dto.SmtpPort;
            setting.EnableSsl = dto.EnableSsl;
            setting.Email = dto.Email.Trim();

            if (!string.IsNullOrEmpty(dto.Password))
            {
                setting.EncryptedPassword = _encryptionService.Encrypt(dto.Password);
            }

            setting.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<MailSettingEntity>().Update(setting);
        }

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("E-posta ayarları başarıyla kaydedildi.");
    }
}

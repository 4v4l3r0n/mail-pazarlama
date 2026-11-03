using MailPazarlama.Business.Common;

namespace MailPazarlama.Business.Services.MailSender;

public interface IMailSenderService
{
    Task<ServiceResult> SendBulkMailAsync(int templateId, int userId);
}

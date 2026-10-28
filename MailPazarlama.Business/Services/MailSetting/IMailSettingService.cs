using MailPazarlama.Business.Common;
using MailPazarlama.Business.DTOs.MailSetting;

namespace MailPazarlama.Business.Services.MailSetting;

public interface IMailSettingService
{
    Task<MailSettingDto?> GetSettingAsync();
    Task<ServiceResult> SaveOrUpdateSettingAsync(MailSettingDto dto);
}

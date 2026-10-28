using MailPazarlama.Business.Common;
using MailPazarlama.Business.DTOs.MailTemplate;

namespace MailPazarlama.Business.Services.MailTemplate;

public interface IMailTemplateService
{
    Task<ServiceResult<MailTemplateDto>> CreateTemplateAsync(MailTemplateCreateDto dto);
    Task<ServiceResult<MailTemplateDto>> UpdateTemplateAsync(MailTemplateUpdateDto dto);
    Task<IReadOnlyList<MailTemplateDto>> GetAllTemplatesAsync();
    Task<IReadOnlyList<MailTemplateDto>> GetActiveTemplatesAsync();
    Task<ServiceResult<MailTemplateDto>> GetByIdAsync(int id);
    Task<ServiceResult> ToggleStatusAsync(int id);
    Task<ServiceResult> DeleteTemplateAsync(int id);
}

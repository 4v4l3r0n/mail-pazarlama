using MailPazarlama.Business.Common;
using MailPazarlama.Business.DTOs.MailTemplate;
using MailPazarlama.Data.UnitOfWork;
using MailPazarlama.Entities;
using MailTemplateEntity = MailPazarlama.Entities.MailTemplate;

namespace MailPazarlama.Business.Services.MailTemplate;

public class MailTemplateService : IMailTemplateService
{
    private readonly IUnitOfWork _unitOfWork;

    public MailTemplateService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<MailTemplateDto>> CreateTemplateAsync(MailTemplateCreateDto dto)
    {
        var template = new MailTemplateEntity
        {
            Title = dto.Title.Trim(),
            ContentHtml = dto.ContentHtml,
            UserId = dto.UserId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<MailTemplateEntity>().AddAsync(template);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<MailTemplateDto>.Ok(MapToDto(template), "Şablon başarıyla oluşturuldu.");
    }

    public async Task<ServiceResult<MailTemplateDto>> UpdateTemplateAsync(MailTemplateUpdateDto dto)
    {
        var template = await _unitOfWork.Repository<MailTemplateEntity>().GetByIdAsync(dto.Id);
        if (template == null)
        {
            return ServiceResult<MailTemplateDto>.Fail("Şablon bulunamadı.");
        }

        template.Title = dto.Title.Trim();
        template.ContentHtml = dto.ContentHtml;

        _unitOfWork.Repository<MailTemplateEntity>().Update(template);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<MailTemplateDto>.Ok(MapToDto(template), "Şablon başarıyla güncellendi.");
    }

    public async Task<IReadOnlyList<MailTemplateDto>> GetAllTemplatesAsync()
    {
        var templates = await _unitOfWork.Repository<MailTemplateEntity>().GetAllAsync();
        return templates.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<MailTemplateDto>> GetActiveTemplatesAsync()
    {
        var templates = await _unitOfWork.Repository<MailTemplateEntity>().FindAsync(t => t.IsActive);
        return templates.Select(MapToDto).ToList();
    }

    public async Task<ServiceResult<MailTemplateDto>> GetByIdAsync(int id)
    {
        var template = await _unitOfWork.Repository<MailTemplateEntity>().GetByIdAsync(id);
        if (template == null)
        {
            return ServiceResult<MailTemplateDto>.Fail("Şablon bulunamadı.");
        }

        return ServiceResult<MailTemplateDto>.Ok(MapToDto(template));
    }

    public async Task<ServiceResult> ToggleStatusAsync(int id)
    {
        var template = await _unitOfWork.Repository<MailTemplateEntity>().GetByIdAsync(id);
        if (template == null)
        {
            return ServiceResult.Fail("Şablon bulunamadı.");
        }

        template.IsActive = !template.IsActive;
        _unitOfWork.Repository<MailTemplateEntity>().Update(template);
        await _unitOfWork.SaveChangesAsync();

        var status = template.IsActive ? "aktif" : "pasif";
        return ServiceResult.Ok($"Şablon durumu {status} yapıldı.");
    }

    public async Task<ServiceResult> DeleteTemplateAsync(int id)
    {
        var template = await _unitOfWork.Repository<MailTemplateEntity>().GetByIdAsync(id);
        if (template == null)
        {
            return ServiceResult.Fail("Şablon bulunamadı.");
        }

        var logs = await _unitOfWork.Repository<MailSendLog>().FindAsync(l => l.TemplateId == id);
        if (logs.Count > 0)
        {
            return ServiceResult.Fail("Daha önce gönderim yapılmış şablon silinemez.");
        }

        _unitOfWork.Repository<MailTemplateEntity>().Delete(template);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Şablon başarıyla silindi.");
    }

    private static MailTemplateDto MapToDto(MailTemplateEntity template)
    {
        return new MailTemplateDto
        {
            Id = template.Id,
            Title = template.Title,
            ContentHtml = template.ContentHtml,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt,
            UserId = template.UserId
        };
    }
}

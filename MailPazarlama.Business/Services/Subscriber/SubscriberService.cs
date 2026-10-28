using MailPazarlama.Business.Common;
using MailPazarlama.Business.DTOs.Subscriber;
using MailPazarlama.Data.UnitOfWork;
using MailPazarlama.Entities;
using SubscriberEntity = MailPazarlama.Entities.Subscriber;

namespace MailPazarlama.Business.Services.Subscriber;

public class SubscriberService : ISubscriberService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubscriberService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<SubscriberDto>> AddSubscriberAsync(SubscriberCreateDto dto)
    {
        var normalizedEmail = dto.Email.Trim().ToLower();
        var existingSubscribers = await _unitOfWork.Repository<SubscriberEntity>()
            .FindAsync(s => s.Email.ToLower() == normalizedEmail);

        if (existingSubscribers.Count > 0)
        {
            return ServiceResult<SubscriberDto>.Fail("Bu e-posta adresi zaten abone.");
        }

        var subscriber = new SubscriberEntity
        {
            Email = normalizedEmail,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<SubscriberEntity>().AddAsync(subscriber);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<SubscriberDto>.Ok(MapToDto(subscriber), "Abone başarıyla eklendi.");
    }

    public async Task<IReadOnlyList<SubscriberDto>> GetAllSubscribersAsync()
    {
        var subscribers = await _unitOfWork.Repository<SubscriberEntity>().GetAllAsync();
        return subscribers.Select(MapToDto).ToList();
    }

    public async Task<ServiceResult> DeleteSubscriberAsync(int id)
    {
        var subscriber = await _unitOfWork.Repository<SubscriberEntity>().GetByIdAsync(id);
        if (subscriber == null)
        {
            return ServiceResult.Fail("Abone bulunamadı.");
        }

        var logs = await _unitOfWork.Repository<MailSendLog>().FindAsync(l => l.SubscriberId == id);
        if (logs.Count > 0)
        {
            return ServiceResult.Fail("Daha önce e-posta gönderilmiş abone silinemez.");
        }

        _unitOfWork.Repository<SubscriberEntity>().Delete(subscriber);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Abone başarıyla silindi.");
    }

    private static SubscriberDto MapToDto(SubscriberEntity subscriber)
    {
        return new SubscriberDto
        {
            Id = subscriber.Id,
            Email = subscriber.Email,
            CreatedAt = subscriber.CreatedAt
        };
    }
}

using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
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
        if (string.IsNullOrWhiteSpace(dto.Email) || !Regex.IsMatch(dto.Email.Trim(), @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
        {
            return ServiceResult<SubscriberDto>.Fail("Geçerli bir e-posta formatı giriniz (örn: isim@alanadi.com).");
        }

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
            CreatedAt = DateTime.Now
        };

        await _unitOfWork.Repository<SubscriberEntity>().AddAsync(subscriber);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<SubscriberDto>.Ok(MapToDto(subscriber), "Abone başarıyla eklendi.");
    }

    public async Task<IReadOnlyList<SubscriberDto>> GetAllSubscribersAsync()
    {
        return await GetSubscribersAsync(null);
    }

    public async Task<IReadOnlyList<SubscriberDto>> GetSubscribersAsync(SubscriberFilterDto? filter)
    {
        var query = _unitOfWork.Repository<SubscriberEntity>().Query();

        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                var search = filter.Email.Trim().ToLower();
                query = query.Where(s => s.Email.ToLower().Contains(search));
            }

            if (filter.StartDate.HasValue)
            {
                var start = filter.StartDate.Value.Date;
                query = query.Where(s => s.CreatedAt >= start);
            }

            if (filter.EndDate.HasValue)
            {
                var endOfDay = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(s => s.CreatedAt <= endOfDay);
            }
        }

        var subscribers = await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
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

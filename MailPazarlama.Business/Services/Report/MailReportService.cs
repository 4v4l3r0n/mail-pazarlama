using Microsoft.EntityFrameworkCore;
using MailPazarlama.Business.DTOs.Report;
using MailPazarlama.Data.UnitOfWork;
using MailPazarlama.Entities;

namespace MailPazarlama.Business.Services.Report;

public class MailReportService : IMailReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public MailReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<MailSendLogDto>> GetLogsAsync(ReportFilterDto filter)
    {
        var query = _unitOfWork.Repository<MailSendLog>().Query()
            .Include(l => l.Subscriber)
            .Include(l => l.MailTemplate)
            .Include(l => l.User)
            .AsQueryable();

        if (filter != null)
        {
            if (filter.TemplateId.HasValue && filter.TemplateId.Value > 0)
            {
                query = query.Where(l => l.TemplateId == filter.TemplateId.Value);
            }

            if (filter.StartDate.HasValue)
            {
                var start = filter.StartDate.Value.Date;
                query = query.Where(l => l.SentAt >= start);
            }

            if (filter.EndDate.HasValue)
            {
                var endOfDay = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(l => l.SentAt <= endOfDay);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status) && filter.Status != "Tümü")
            {
                query = query.Where(l => l.Status == filter.Status);
            }
        }

        var logs = await query.OrderByDescending(l => l.SentAt).ToListAsync();

        return logs.Select(l => new MailSendLogDto
        {
            Id = l.Id,
            SubscriberId = l.SubscriberId,
            SubscriberEmail = l.Subscriber?.Email ?? string.Empty,
            TemplateId = l.TemplateId,
            TemplateTitle = l.MailTemplate?.Title ?? string.Empty,
            UserId = l.UserId,
            UserFullName = l.User != null ? $"{l.User.FirstName} {l.User.LastName}".Trim() : string.Empty,
            SentAt = l.SentAt,
            Status = l.Status,
            ErrorMessage = l.ErrorMessage
        }).ToList();
    }
}

using MailPazarlama.Business.DTOs.Report;

namespace MailPazarlama.Business.Services.Report;

public interface IMailReportService
{
    Task<IReadOnlyList<MailSendLogDto>> GetLogsAsync(ReportFilterDto filter);
}

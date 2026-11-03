using MailPazarlama.Business.DTOs.MailTemplate;
using MailPazarlama.Business.DTOs.Report;

namespace MailPazarlama.Web.Models;

public class ReportsViewModel
{
    public ReportFilterDto Filter { get; set; } = new();
    public IReadOnlyList<MailSendLogDto> Logs { get; set; } = new List<MailSendLogDto>();
    public IReadOnlyList<MailTemplateDto> Templates { get; set; } = new List<MailTemplateDto>();
}

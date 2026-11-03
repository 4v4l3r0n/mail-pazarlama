using MailPazarlama.Business.DTOs.MailTemplate;

namespace MailPazarlama.Web.Models;

public class SendMailViewModel
{
    public int SubscriberCount { get; set; }
    public IReadOnlyList<MailTemplateDto> ActiveTemplates { get; set; } = new List<MailTemplateDto>();
    public int SelectedTemplateId { get; set; }
}

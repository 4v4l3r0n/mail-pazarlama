using MailPazarlama.Business.DTOs.MailTemplate;
using MailPazarlama.Business.DTOs.Subscriber;

namespace MailPazarlama.Web.Models;

public class SendMailViewModel
{
    public IReadOnlyList<MailTemplateDto> ActiveTemplates { get; set; } = new List<MailTemplateDto>();
    public IReadOnlyList<SubscriberDto> Subscribers { get; set; } = new List<SubscriberDto>();
}

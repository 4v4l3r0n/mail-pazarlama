using MailPazarlama.Business.DTOs.Subscriber;

namespace MailPazarlama.Web.Models;

public class SubscribersViewModel
{
    public SubscriberFilterDto Filter { get; set; } = new();
    public IReadOnlyList<SubscriberDto> Subscribers { get; set; } = new List<SubscriberDto>();
}


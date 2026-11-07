using MailPazarlama.Business.Common;
using MailPazarlama.Business.DTOs.Subscriber;

namespace MailPazarlama.Business.Services.Subscriber;

public interface ISubscriberService
{
    Task<ServiceResult<SubscriberDto>> AddSubscriberAsync(SubscriberCreateDto dto);
    Task<IReadOnlyList<SubscriberDto>> GetAllSubscribersAsync();
    Task<IReadOnlyList<SubscriberDto>> GetSubscribersAsync(SubscriberFilterDto? filter);
    Task<ServiceResult> DeleteSubscriberAsync(int id);
}

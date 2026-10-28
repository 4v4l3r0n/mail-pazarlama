namespace MailPazarlama.Business.DTOs.Subscriber;

public class SubscriberDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

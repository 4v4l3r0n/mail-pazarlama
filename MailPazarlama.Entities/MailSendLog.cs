namespace MailPazarlama.Entities;

public class MailSendLog
{
    public int Id { get; set; }
    public int SubscriberId { get; set; }
    public int TemplateId { get; set; }
    public int UserId { get; set; }
    public DateTime SentAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }

    // Navigation Properties
    public Subscriber Subscriber { get; set; } = null!;
    public MailTemplate MailTemplate { get; set; } = null!;
    public User User { get; set; } = null!;
}

namespace MailPazarlama.Entities;

public class Subscriber
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Navigation Property
    public ICollection<MailSendLog> MailSendLogs { get; set; } = new List<MailSendLog>();
}

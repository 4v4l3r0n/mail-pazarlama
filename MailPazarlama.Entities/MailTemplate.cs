namespace MailPazarlama.Entities;

public class MailTemplate
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentHtml { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public ICollection<MailSendLog> MailSendLogs { get; set; } = new List<MailSendLog>();
}

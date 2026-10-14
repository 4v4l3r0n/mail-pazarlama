namespace MailPazarlama.Entities;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public ICollection<MailTemplate> MailTemplates { get; set; } = new List<MailTemplate>();
    public ICollection<MailSendLog> MailSendLogs { get; set; } = new List<MailSendLog>();
}

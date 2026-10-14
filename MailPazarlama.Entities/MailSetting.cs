namespace MailPazarlama.Entities;

public class MailSetting
{
    public int Id { get; set; }
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public bool EnableSsl { get; set; } = true;
    public string Email { get; set; } = string.Empty;
    public string EncryptedPassword { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}

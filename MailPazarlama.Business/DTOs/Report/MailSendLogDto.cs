namespace MailPazarlama.Business.DTOs.Report;

public class MailSendLogDto
{
    public int Id { get; set; }
    public int SubscriberId { get; set; }
    public string SubscriberEmail { get; set; } = string.Empty;
    public int TemplateId { get; set; }
    public string TemplateTitle { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

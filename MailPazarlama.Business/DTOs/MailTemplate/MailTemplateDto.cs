namespace MailPazarlama.Business.DTOs.MailTemplate;

public class MailTemplateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ContentHtml { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }
}

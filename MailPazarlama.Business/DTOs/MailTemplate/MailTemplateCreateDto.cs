using System.ComponentModel.DataAnnotations;

namespace MailPazarlama.Business.DTOs.MailTemplate;

public class MailTemplateCreateDto
{
    [Required(ErrorMessage = "Başlık alanı zorunludur.")]
    [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "İçerik alanı zorunludur.")]
    public string ContentHtml { get; set; } = string.Empty;

    public int UserId { get; set; }
}

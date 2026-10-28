using System.ComponentModel.DataAnnotations;

namespace MailPazarlama.Business.DTOs.MailSetting;

public class MailSettingDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "SMTP sunucu adresi zorunludur.")]
    public string SmtpServer { get; set; } = string.Empty;

    [Required(ErrorMessage = "SMTP port zorunludur.")]
    [Range(1, 65535, ErrorMessage = "Geçerli bir port numarası giriniz.")]
    public int SmtpPort { get; set; }

    public bool EnableSsl { get; set; } = true;

    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}

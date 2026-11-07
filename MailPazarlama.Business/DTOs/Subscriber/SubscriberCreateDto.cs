using System.ComponentModel.DataAnnotations;

namespace MailPazarlama.Business.DTOs.Subscriber;

public class SubscriberCreateDto
{
    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Geçerli bir e-posta formatı giriniz (örn: isim@alanadi.com).")]
    public string Email { get; set; } = string.Empty;
}

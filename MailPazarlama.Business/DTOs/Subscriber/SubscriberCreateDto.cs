using System.ComponentModel.DataAnnotations;

namespace MailPazarlama.Business.DTOs.Subscriber;

public class SubscriberCreateDto
{
    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;
}

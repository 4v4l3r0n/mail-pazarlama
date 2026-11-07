using System.ComponentModel.DataAnnotations;

namespace MailPazarlama.Web.Models;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; } = false;

    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    public string? ConfirmNewPassword { get; set; }
}

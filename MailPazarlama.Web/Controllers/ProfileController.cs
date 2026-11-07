using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.DTOs.User;
using MailPazarlama.Business.Services.User;

namespace MailPazarlama.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IUserService _userService;

    public ProfileController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId))
        {
            TempData["Error"] = "Kullanıcı bilgisi alınamadı.";
            return RedirectToAction("Index", "Home");
        }

        var result = await _userService.GetByIdAsync(userId);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = "Kullanıcı bulunamadı.";
            return RedirectToAction("Index", "Home");
        }

        var dto = new UserProfileDto
        {
            Id = result.Data.Id,
            FirstName = result.Data.FirstName,
            LastName = result.Data.LastName,
            Email = result.Data.Email,
            Password = string.Empty
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(UserProfileDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId))
        {
            TempData["Error"] = "Kullanıcı kimliği doğrulanamadı.";
            return RedirectToAction("Index", "Home");
        }

        var result = await _userService.UpdateProfileAsync(userId, dto);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(dto);
        }

        // Kimlik doğrulama çerezini güncel isim ve e-posta ile yenile
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, $"{dto.FirstName.Trim()} {dto.LastName.Trim()}"),
            new Claim(ClaimTypes.Email, dto.Email.Trim().ToLower())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        TempData["Success"] = "Profil bilgileriniz başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }
}

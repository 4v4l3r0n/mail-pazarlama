using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.DTOs.User;
using MailPazarlama.Business.Services.User;

namespace MailPazarlama.Web.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new UserLoginDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginDto dto, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var result = await _userService.LoginAsync(dto);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = result.Message;
            ModelState.AddModelError(string.Empty, result.Message);
            return View(dto);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.Data.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{result.Data.FirstName} {result.Data.LastName}"),
            new Claim(ClaimTypes.Email, result.Data.Email)
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

        TempData["Success"] = $"Hoş geldiniz, {result.Data.FirstName} {result.Data.LastName}!";
        return RedirectToLocal(returnUrl);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Subscribers");
        }

        return View(new UserRegisterDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var result = await _userService.RegisterAsync(dto);
        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            ModelState.AddModelError(string.Empty, result.Message);
            return View(dto);
        }

        TempData["Success"] = result.Message + " Lütfen giriş yapınız.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Success"] = "Başarıyla çıkış yapıldı.";
        return RedirectToAction(nameof(Login));
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Subscribers");
    }
}

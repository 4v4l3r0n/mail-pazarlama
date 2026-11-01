using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.DTOs.MailSetting;
using MailPazarlama.Business.Services.MailSetting;

namespace MailPazarlama.Web.Controllers;

[Authorize]
public class SettingsController : Controller
{
    private readonly IMailSettingService _settingService;

    public SettingsController(IMailSettingService settingService)
    {
        _settingService = settingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var setting = await _settingService.GetSettingAsync();
        return View(setting ?? new MailSettingDto { SmtpPort = 587, EnableSsl = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(MailSettingDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var result = await _settingService.SaveOrUpdateSettingAsync(dto);
        if (result.Success)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = result.Message;
        return View(dto);
    }
}

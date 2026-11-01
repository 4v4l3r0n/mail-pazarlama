using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.DTOs.Subscriber;
using MailPazarlama.Business.Services.Subscriber;

namespace MailPazarlama.Web.Controllers;

[Authorize]
public class SubscribersController : Controller
{
    private readonly ISubscriberService _subscriberService;

    public SubscribersController(ISubscriberService subscriberService)
    {
        _subscriberService = subscriberService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var subscribers = await _subscriberService.GetAllSubscribersAsync();
        return View(subscribers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubscriberCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Lütfen geçerli bir e-posta adresi giriniz.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _subscriberService.AddSubscriberAsync(dto);
        if (result.Success)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _subscriberService.DeleteSubscriberAsync(id);
        if (result.Success)
        {
            TempData["Success"] = result.Message;
        }
        else
        {
            TempData["Error"] = result.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

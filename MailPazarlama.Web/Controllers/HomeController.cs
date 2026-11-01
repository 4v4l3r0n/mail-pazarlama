using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.DTOs.Subscriber;
using MailPazarlama.Business.Services.Subscriber;

namespace MailPazarlama.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ISubscriberService _subscriberService;

    public HomeController(ISubscriberService subscriberService)
    {
        _subscriberService = subscriberService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new SubscriberCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SubscriberCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var result = await _subscriberService.AddSubscriberAsync(dto);
        if (result.Success)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = result.Message;
        return View(dto);
    }
}

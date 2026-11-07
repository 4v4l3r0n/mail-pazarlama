using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.DTOs.Subscriber;
using MailPazarlama.Business.Services.Subscriber;
using MailPazarlama.Web.Models;

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
    public async Task<IActionResult> Index([FromQuery] SubscriberFilterDto? filter)
    {
        filter ??= new SubscriberFilterDto();
        var subscribers = await _subscriberService.GetSubscribersAsync(filter);

        var viewModel = new SubscribersViewModel
        {
            Filter = filter,
            Subscribers = subscribers
        };

        return View(viewModel);
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

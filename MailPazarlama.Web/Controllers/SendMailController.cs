using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.Services.MailSender;
using MailPazarlama.Business.Services.MailTemplate;
using MailPazarlama.Business.Services.Subscriber;
using MailPazarlama.Web.Models;

namespace MailPazarlama.Web.Controllers;

[Authorize]
public class SendMailController : Controller
{
    private readonly IMailSenderService _mailSenderService;
    private readonly IMailTemplateService _mailTemplateService;
    private readonly ISubscriberService _subscriberService;

    public SendMailController(
        IMailSenderService mailSenderService,
        IMailTemplateService mailTemplateService,
        ISubscriberService subscriberService)
    {
        _mailSenderService = mailSenderService;
        _mailTemplateService = mailTemplateService;
        _subscriberService = subscriberService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var activeTemplates = await _mailTemplateService.GetActiveTemplatesAsync();
        var subscribers = await _subscriberService.GetAllSubscribersAsync();

        var viewModel = new SendMailViewModel
        {
            ActiveTemplates = activeTemplates,
            Subscribers = subscribers
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(int templateId, List<int> selectedSubscriberIds)
    {
        if (templateId <= 0)
        {
            TempData["Error"] = "Lütfen gönderim yapılacak geçerli bir şablon seçiniz.";
            return RedirectToAction(nameof(Index));
        }

        if (selectedSubscriberIds == null || selectedSubscriberIds.Count == 0)
        {
            TempData["Error"] = "Lütfen gönderim yapılacak en az bir abone seçiniz.";
            return RedirectToAction(nameof(Index));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
        {
            TempData["Error"] = "Kullanıcı kimliği doğrulanamadı.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _mailSenderService.SendBulkMailAsync(templateId, selectedSubscriberIds, userId);
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

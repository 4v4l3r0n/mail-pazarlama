using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.DTOs.Report;
using MailPazarlama.Business.DTOs.Subscriber;
using MailPazarlama.Business.Services.MailTemplate;
using MailPazarlama.Business.Services.Report;
using MailPazarlama.Business.Services.Subscriber;
using MailPazarlama.Web.Models;

namespace MailPazarlama.Web.Controllers;

[AllowAnonymous]
public class HomeController : Controller
{
    private readonly ISubscriberService _subscriberService;
    private readonly IMailTemplateService _mailTemplateService;
    private readonly IMailReportService _mailReportService;

    public HomeController(
        ISubscriberService subscriberService,
        IMailTemplateService mailTemplateService,
        IMailReportService mailReportService)
    {
        _subscriberService = subscriberService;
        _mailTemplateService = mailTemplateService;
        _mailReportService = mailReportService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var subscribers = await _subscriberService.GetAllSubscribersAsync();
            var templates = await _mailTemplateService.GetAllTemplatesAsync();
            var logs = await _mailReportService.GetLogsAsync(new ReportFilterDto());

            var viewModel = new DashboardViewModel
            {
                UserName = User.Identity.Name ?? "Kullanıcı",
                SubscriberCount = subscribers.Count,
                TemplateCount = templates.Count,
                SentMailCount = logs.Count
            };

            return View("Dashboard", viewModel);
        }

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

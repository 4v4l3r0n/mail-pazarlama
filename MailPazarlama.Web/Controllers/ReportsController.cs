using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.DTOs.Report;
using MailPazarlama.Business.Services.MailTemplate;
using MailPazarlama.Business.Services.Report;
using MailPazarlama.Web.Models;

namespace MailPazarlama.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IMailReportService _reportService;
    private readonly IMailTemplateService _templateService;

    public ReportsController(
        IMailReportService reportService,
        IMailTemplateService templateService)
    {
        _reportService = reportService;
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] ReportFilterDto filter)
    {
        filter ??= new ReportFilterDto();

        var logs = await _reportService.GetLogsAsync(filter);
        var templates = await _templateService.GetAllTemplatesAsync();

        var viewModel = new ReportsViewModel
        {
            Filter = filter,
            Logs = logs,
            Templates = templates
        };

        return View(viewModel);
    }
}

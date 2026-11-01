using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.DTOs.MailTemplate;
using MailPazarlama.Business.Services.MailTemplate;

namespace MailPazarlama.Web.Controllers;

[Authorize]
public class TemplatesController : Controller
{
    private readonly IMailTemplateService _templateService;

    public TemplatesController(IMailTemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var templates = await _templateService.GetAllTemplatesAsync();
        return View(templates);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new MailTemplateCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MailTemplateCreateDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int userId))
        {
            dto.UserId = userId;
        }

        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var result = await _templateService.CreateTemplateAsync(dto);
        if (result.Success)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = result.Message;
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _templateService.GetByIdAsync(id);
        if (!result.Success || result.Data == null)
        {
            TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        var updateDto = new MailTemplateUpdateDto
        {
            Id = result.Data.Id,
            Title = result.Data.Title,
            ContentHtml = result.Data.ContentHtml
        };

        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MailTemplateUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var result = await _templateService.UpdateTemplateAsync(dto);
        if (result.Success)
        {
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = result.Message;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _templateService.ToggleStatusAsync(id);
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
        var result = await _templateService.DeleteTemplateAsync(id);
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

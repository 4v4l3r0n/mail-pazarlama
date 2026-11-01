using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MailPazarlama.Business.Services.User;

namespace MailPazarlama.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await _userService.ToggleStatusAsync(id);
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

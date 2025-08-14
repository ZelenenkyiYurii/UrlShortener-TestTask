using Microsoft.AspNetCore.Mvc;
using UrlShortener.Web.Services;

namespace UrlShortener.Web.Controllers;

public class AboutController : Controller
{
    private readonly IAboutContentService _aboutService;

    public AboutController(IAboutContentService aboutService)
    {
        _aboutService = aboutService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var content = await _aboutService.GetCurrentContentAsync();
        return View(content);
    }

    [HttpPost]
    public async Task<IActionResult> Update(string title, string content)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var role = HttpContext.Session.GetString("Role");
        if (userId == null || role != "Admin") return Unauthorized();

        await _aboutService.UpdateContentAsync(title, content, userId.Value);
        return RedirectToAction(nameof(Index));
    }
}
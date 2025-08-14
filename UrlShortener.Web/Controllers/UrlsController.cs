using Microsoft.AspNetCore.Mvc;
using UrlShortener.Web.Services;

namespace UrlShortener.Web.Controllers;

public class UrlsController : Controller
{
    private readonly IUrlShorteningService _urlService;

    public UrlsController(IUrlShorteningService urlService)
    {
        _urlService = urlService;
    }

    public async Task<IActionResult> Index()
    {
        var urls = await _urlService.GetAllUrlsAsync();
        return View(urls); 
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if(userId is null) 
            return RedirectToAction("Login", "Account",new {returnUrl=Url.Action(nameof(Details),new{id})});

        var u = await _urlService.GetUrlByIdAsync(id);
        if(u is null) return NotFound();
        
        return View(u);
    }
}
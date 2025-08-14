using Microsoft.AspNetCore.Mvc;
using UrlShortener.Web.Services;

namespace UrlShortener.Web.Controllers;

public class RedirectController:Controller
{
    private readonly IUrlShorteningService _urlService;

    public RedirectController(IUrlShorteningService urlService)
    {
        _urlService = urlService;
    }

    [HttpGet("/u/{code}")]
    public async Task<IActionResult> Go(string code)
    {
        var targetUrl = await _urlService.RedirectToOriginalUrlAsync(code);
        if (string.IsNullOrEmpty(targetUrl))
            return NotFound();

        return Redirect(targetUrl);
    }
}
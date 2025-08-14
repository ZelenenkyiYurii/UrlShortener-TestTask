using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Web.Models;

namespace UrlShortener.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("HomeController.Index called");
        return View();
    }
}
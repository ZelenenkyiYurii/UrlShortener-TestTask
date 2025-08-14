using System.Net;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Web.Models.DTOs;
using UrlShortener.Web.Services;

namespace UrlShortener.Web.Controllers;

[ApiController]
[Route("api/urls")]
public class UrlsApiController:ControllerBase
{
    private readonly IUrlShorteningService _urlService;

    public UrlsApiController(IUrlShorteningService urlService)
    {
        _urlService = urlService;
        
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _urlService.GetAllUrlsAsync();
        
        var userId=HttpContext.Session.GetInt32("UserId");
        var role = HttpContext.Session.GetString("Role");
        var isAuthorized = userId.HasValue;
        var isAdmin=string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);

        var dto = new UrlsPageDto
        {
            IsAuthorized = isAuthorized,
            IsAdmin = isAdmin,
            CurrentUserId = userId
        };

        dto.Items = list.Select(u => new UrlListItemDto
        {
            Id = u.Id,
            OriginalUrl = u.OriginalUrl,
            ShortCode = u.ShortCode,
            CreatedById = u.CreatedById,
            CanDelete = isAuthorized && (isAdmin || u.CreatedById==userId)
        }).ToList();
        return Ok(dto);
    }
    [HttpPost]
    public async Task<ActionResult<UrlListItemDto>> Create([FromBody] CreateUrlRequest req)
    {
        try
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId is null) return Unauthorized();

            if (string.IsNullOrWhiteSpace(req.OriginalUrl)) return BadRequest("Original URL is empty");

            if (await _urlService.OriginalUrlExistsAsync(req.OriginalUrl))
                return Conflict("Such URL already exists");

            var created = await _urlService.CreateShortUrlAsync(req.OriginalUrl, userId.Value);

            var role = HttpContext.Session.GetString("Role");
            var isAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);

            var dto = new UrlListItemDto
            {
                Id = created.Id,
                OriginalUrl = created.OriginalUrl,
                ShortCode = created.ShortCode,
                CreatedById = created.CreatedById,
                CanDelete = isAdmin || created.CreatedById == userId
            };

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UrlListItemDto>> GetById(int id)
    {
        var u=await  _urlService.GetUrlByIdAsync(id);
        if(u is null) return NotFound();
        
        var userId = HttpContext.Session.GetInt32("UserId");
        var role = HttpContext.Session.GetString("Role");
        var isAuthorized = userId.HasValue;
        var isAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
        
        var dto = new UrlListItemDto
        {
            Id = u.Id,
            OriginalUrl = u.OriginalUrl,
            ShortCode = u.ShortCode,
            CreatedById = u.CreatedById,
            CanDelete = isAuthorized && (isAdmin || u.CreatedById == userId)
        };

        return Ok(dto);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId=HttpContext.Session.GetInt32("UserId");
        if(userId is null) return Unauthorized();
        
        var role = HttpContext.Session.GetString("Role");
        var isAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);

        var ok = await _urlService.DeleteUrlAsync(id, userId.Value,isAdmin);
        return ok? NoContent() : Forbid();
        
    }
    
}
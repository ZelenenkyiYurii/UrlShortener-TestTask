namespace UrlShortener.Web.Models.DTOs;

public class UrlListItemDto
{
    public int Id { get; set; }
    public string OriginalUrl { get; set; } = "";
    public string ShortCode { get; set; } = "";
    public int CreatedById { get; set; }
    public int Clicks { get; set; }
    public bool CanDelete { get; set; } // ← для кнопки Delete
}

public class UrlsPageDto
{
    public bool IsAuthorized { get; set; }
    public bool IsAdmin { get; set; }
    public int? CurrentUserId { get; set; }
    public List<UrlListItemDto> Items { get; set; } = new();
}

public class CreateUrlRequest
{
    public string OriginalUrl { get; set; } = "";
}
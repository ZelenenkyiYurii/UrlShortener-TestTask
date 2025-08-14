namespace UrlShortener.Web.Models.Domain;

public class UrlMapping
{
    public int Id { get; set; }
    public string OriginalUrl { get; set; }

    public string ShortCode { get; set; }
    
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public int CreatedById { get; set; }

    public virtual User CreatedBy { get; set; } = null!;

}
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Web.Models.Domain;

public class AboutContent
{
    public int Id { get; set; } = 1;
    
    [Required]
    public string Title { get; set; } = "Url Shortener Algorithm";
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
}
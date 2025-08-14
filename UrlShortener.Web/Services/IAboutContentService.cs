using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Services;

public interface IAboutContentService
{ 
    Task<AboutContent?> GetCurrentContentAsync(); 
    Task<AboutContent> UpdateContentAsync(string title, string content, int userId);
}
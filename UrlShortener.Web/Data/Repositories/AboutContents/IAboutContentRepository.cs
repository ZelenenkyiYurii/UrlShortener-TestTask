using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Data.Repositories.AboutContents;

public interface IAboutContentRepository
{
    Task<AboutContent?> GetCurrentAsync();
    Task<AboutContent> UpdateAsync(AboutContent aboutContent);
    Task<AboutContent> CreateAsync(AboutContent aboutContent);
}
using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Services;

public interface IUrlShorteningService
{
    Task<UrlMapping> CreateShortUrlAsync(string originalUrl, int userId);
    Task<UrlMapping?> GetUrlByShortCodeAsync(string shortCode);
    Task<UrlMapping?> GetUrlByIdAsync(int id);
    Task<List<UrlMapping>> GetAllUrlsAsync();
    Task<List<UrlMapping>> GetUserUrlsAsync(int userId);
    Task<bool> DeleteUrlAsync(int urlId, int userId, bool isAdmin = false);
    Task<bool> OriginalUrlExistsAsync(string originalUrl);
    Task<string> RedirectToOriginalUrlAsync(string shortCode);
    string GenerateShortCode(int id);
}
using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Data.Repositories.UrlMappings;

public interface IUrlMappingRepository
{
    Task<UrlMapping?> GetByIdAsync(int id);
    Task<UrlMapping?> GetByShortCodeAsync(string shortCode);
    Task<UrlMapping?> GetByOriginalUrlAsync(string originalUrl);
    Task<List<UrlMapping>> GetAllAsync();
    Task<List<UrlMapping>> GetByUserIdAsync(int userId);
    Task<UrlMapping> CreateAsync(UrlMapping urlMapping);
    
    Task<UrlMapping> UpdateAsync(UrlMapping urlMapping);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeleteByUserIdAsync(int id, int userId); // тільки свої URL
    Task<bool> ShortCodeExistsAsync(string shortCode);
    Task<bool> OriginalUrlExistsAsync(string originalUrl);
    
}
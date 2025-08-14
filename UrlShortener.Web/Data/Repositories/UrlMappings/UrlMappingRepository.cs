using Microsoft.EntityFrameworkCore;
using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Data.Repositories.UrlMappings;

public class UrlMappingRepository:IUrlMappingRepository
{
    private readonly UrlShortenerDbContext _context;

    public UrlMappingRepository(UrlShortenerDbContext context)
    {
        _context = context;
    }
    
    public async Task<UrlMapping?> GetByIdAsync(int id)
    {
        return await _context.UrlMappings
            .Include(u=>u.CreatedBy)
            .FirstOrDefaultAsync(u=>u.Id == id);
    }

    public async Task<UrlMapping?> GetByShortCodeAsync(string shortCode)
    {
        return await _context.UrlMappings
            .Include(u => u.CreatedBy)
            .FirstOrDefaultAsync(u => u.ShortCode == shortCode);
    }

    public async Task<UrlMapping?> GetByOriginalUrlAsync(string originalUrl)
    {
        return await _context.UrlMappings
            .Include(u => u.CreatedBy)
            .FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl);
    }

    public async Task<List<UrlMapping>> GetAllAsync()
    {
        return await _context.UrlMappings
            .Include(u => u.CreatedBy)
            .OrderByDescending(u => u.CreatedOn)
            .ToListAsync();
    }

    public async Task<List<UrlMapping>> GetByUserIdAsync(int userId)
    {
        return await _context.UrlMappings
            .Include(u => u.CreatedBy)
            .Where(u => u.CreatedById == userId)
            .OrderByDescending(u => u.CreatedOn)
            .ToListAsync();
    }

    public async Task<UrlMapping> CreateAsync(UrlMapping urlMapping)
    {
        _context.UrlMappings.Add(urlMapping);
        await _context.SaveChangesAsync();
        
        return await GetByIdAsync(urlMapping.Id)?? throw new InvalidOperationException("Failed to create url mapping");
    }

    public async Task<UrlMapping> UpdateAsync(UrlMapping urlMapping)
    {
        _context.UrlMappings.Update(urlMapping);
        await _context.SaveChangesAsync();
        return urlMapping;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var urlMapping = await _context.UrlMappings.FindAsync(id);
        if (urlMapping == null) return false;
        
        _context.UrlMappings.Remove(urlMapping);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteByUserIdAsync(int id, int userId)
    {
        var urlMapping = await _context.UrlMappings
            .FirstOrDefaultAsync(u=>u.Id == id && u.CreatedById==userId);
        
        if(urlMapping == null) return false;
        
        _context.UrlMappings.Remove(urlMapping);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ShortCodeExistsAsync(string shortCode)
    {
        return await _context.UrlMappings.AnyAsync(u => u.ShortCode == shortCode);
    }

    public async Task<bool> OriginalUrlExistsAsync(string originalUrl)
    {
        return await _context.UrlMappings.AnyAsync(u => u.OriginalUrl == originalUrl);
    }
}
using Microsoft.EntityFrameworkCore;
using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Data.Repositories.AboutContents;

public class AboutContentRepository: IAboutContentRepository
{
    private readonly UrlShortenerDbContext _context;

    public AboutContentRepository(UrlShortenerDbContext context)
    {
        _context = context;
    }
    public async Task<AboutContent?> GetCurrentAsync()
    {
        return await _context.AboutContents.FirstOrDefaultAsync();
    }

    public async Task<AboutContent> UpdateAsync(AboutContent aboutContent)
    {
       var existing = await _context.AboutContents.FirstOrDefaultAsync();
       if (existing != null)
       {
           existing.Title = aboutContent.Title;
           existing.Content = aboutContent.Content;
           
           await _context.SaveChangesAsync();
                  return existing;
       }
       return await CreateAsync(aboutContent);
    }

    public async Task<AboutContent> CreateAsync(AboutContent aboutContent)
    {
        _context.AboutContents.Add(aboutContent);
        await _context.SaveChangesAsync();
        return aboutContent;
    }
}
using UrlShortener.Web.Data.Repositories.AboutContents;
using UrlShortener.Web.Data.Repositories.Users;
using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Services;

public class AboutContentService:IAboutContentService
{
    private readonly IAboutContentRepository _aboutRepository;
    private readonly IUserRepository _userRepository;

    public AboutContentService(IAboutContentRepository aboutRepository, IUserRepository userRepository)
    {
        _aboutRepository = aboutRepository;
        _userRepository = userRepository;
    }

    public async Task<AboutContent?> GetCurrentContentAsync()
    {
        return await _aboutRepository.GetCurrentAsync();
    }

    public async Task<AboutContent> UpdateContentAsync(string title, string content, int userId)
    {
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found.", nameof(userId));
        }

        if (user.Role != UserRole.Admin)
        {
            throw new UnauthorizedAccessException("Only administrators can update the About content.");
        }

        var aboutContent = new AboutContent
        {
            Title = title.Trim(),
            Content = content.Trim()
        };

        return await _aboutRepository.UpdateAsync(aboutContent);
    }
}
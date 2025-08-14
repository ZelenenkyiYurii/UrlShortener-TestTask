using System.Text.RegularExpressions;
using UrlShortener.Web.Data.Repositories.UrlMappings;
using UrlShortener.Web.Data.Repositories.Users;
using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Services;

public class UrlShorteningService:IUrlShorteningService
{
    
    private readonly IUrlMappingRepository _urlRepository;
    private readonly IUserRepository _userRepository;
    
    private const string Base62Chars ="0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    
    private const long Multiplier = 15485863; 
    private const long Salt = 987654321; 
    
    private static readonly Regex UrlRegex = new Regex(
        @"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    private static readonly Regex ProtocolRegex = new Regex(
        @"^https?:\/\/", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public UrlShorteningService(IUrlMappingRepository urlRepository, IUserRepository userRepository)
    {
        _urlRepository = urlRepository;
        _userRepository = userRepository;
        
    }
    public async Task<UrlMapping> CreateShortUrlAsync(string originalUrl, int userId)
    {
        var normalizedUrl=ValidateAndNormalizeUrl(originalUrl);
        
        var existingUrl = await _urlRepository.GetByOriginalUrlAsync(normalizedUrl);
        if (existingUrl != null)
        {
            throw new InvalidOperationException("Url already exists");
        }
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found", nameof(userId));
        }

        var urlMapping = new UrlMapping
        {
            OriginalUrl = normalizedUrl,
            ShortCode = "temp",
            CreatedById = userId,
            CreatedOn = DateTime.UtcNow
        };
        
        var savedUrl=await _urlRepository.CreateAsync(urlMapping);

        string shortCode;
        int attempts = 0;
        do
        {
            shortCode=GenerateShortCode(savedUrl.Id+attempts);
            attempts++;
        }while(await _urlRepository.ShortCodeExistsAsync(shortCode) && attempts<10);

        if (attempts >= 10)
        {
            throw new InvalidOperationException("Failed to generate unique short code.");
        }
        
        savedUrl.ShortCode=shortCode;
        await _urlRepository.UpdateAsync(savedUrl);
        
        return savedUrl;
    }

    public async Task<UrlMapping?> GetUrlByShortCodeAsync(string shortCode)
    {
        return await _urlRepository.GetByShortCodeAsync(shortCode);
    }

    public async Task<UrlMapping?> GetUrlByIdAsync(int id)
    {
        return await _urlRepository.GetByIdAsync(id);
    }

    public async Task<List<UrlMapping>> GetAllUrlsAsync()
    {
        return await _urlRepository.GetAllAsync();
    }

    public async Task<List<UrlMapping>> GetUserUrlsAsync(int userId)
    {
        return await _urlRepository.GetByUserIdAsync(userId);
    }

    public async Task<bool> DeleteUrlAsync(int urlId, int userId, bool isAdmin = false)
    {
        if (isAdmin)
        {
            return await _urlRepository.DeleteAsync(urlId);
        }
        else
        {
            return await _urlRepository.DeleteByUserIdAsync(urlId, userId);
        }
    }

    public async Task<bool> OriginalUrlExistsAsync(string originalUrl)
    {
        try
        {
            var normalizedUrl = ValidateAndNormalizeUrl(originalUrl);
            return await _urlRepository.OriginalUrlExistsAsync(normalizedUrl);
        }
        catch (ArgumentException)
        {
            
            return false;
        }
    }

    public async Task<string> RedirectToOriginalUrlAsync(string shortCode)
    {
        var urlMapping = await _urlRepository.GetByShortCodeAsync(shortCode);
            
        if (urlMapping == null)
        {
            throw new ArgumentException($"Short code '{shortCode}' not found.", nameof(shortCode));
        }
        
        return urlMapping.OriginalUrl;
    }

    

    public string GenerateShortCode(int id)
    {
        long number = (long)id + Salt;
        number *= Multiplier;
        
        var result = "";
        while (number > 0)
        {
            result = Base62Chars[(int)(number % 62)] + result;
            number /= 62;
        }
        const int minLength = 4;
        if (result.Length < minLength)
        {
            var padding = new System.Text.StringBuilder();
            for(int i = 0; i < minLength - result.Length; i++)
            {
                padding.Append(Base62Chars[(id + i) % Base62Chars.Length]);
            }
            result = padding.ToString() + result;
        }
        const int maxLength = 10;
        if(result.Length > maxLength)
        {
            result = result.Substring(0, maxLength);
        }
        return result;
    }
    
    private static string ValidateAndNormalizeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("URL cannot be empty.", nameof(url));
        }

        url = url.Trim();
        
        if (!ProtocolRegex.IsMatch(url))
        {
            url = "https://" + url;
        }

        if (!UrlRegex.IsMatch(url))
        {
            throw new ArgumentException("Invalid URL format. Please provide a valid web address.", nameof(url));
        }
        
        if (url.Length > 2048)
        {
            throw new ArgumentException("URL is too long. Maximum length is 2048 characters.", nameof(url));
        }

        return url;
    }
}
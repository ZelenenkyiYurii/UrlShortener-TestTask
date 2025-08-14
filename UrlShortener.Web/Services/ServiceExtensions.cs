using Microsoft.EntityFrameworkCore;
using UrlShortener.Web.Data;
using UrlShortener.Web.Data.Repositories.AboutContents;
using UrlShortener.Web.Data.Repositories.UrlMappings;
using UrlShortener.Web.Data.Repositories.Users;

namespace UrlShortener.Web.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<UrlShortenerDbContext>(options =>
            options.UseSqlite(config.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IUrlMappingRepository, UrlMappingRepository>(); 
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAboutContentRepository, AboutContentRepository>();
                
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUrlShorteningService, UrlShorteningService>();
        services.AddScoped<IAboutContentService, AboutContentService>();
        
        
        
        return services;
    }
}
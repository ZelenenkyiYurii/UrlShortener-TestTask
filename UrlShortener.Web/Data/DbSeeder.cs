using UrlShortener.Web.Models.Domain;
using UrlShortener.Web.Services;

namespace UrlShortener.Web.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UrlShortenerDbContext>();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

            // Якщо вже є користувачі — сидер не запускаємо повторно
            if (!db.Users.Any())
            {
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = authService.HashPassword("admin"),
                    Role = UserRole.Admin
                };

                var user1 = new User
                {
                    Username = "user1",
                    PasswordHash = authService.HashPassword("1111"),
                    Role = UserRole.User
                };
                var user2 = new User
                {
                    Username = "user2",
                    PasswordHash = authService.HashPassword("2222"),
                    Role = UserRole.User
                };

                db.Users.AddRange(admin, user1, user2);
                await db.SaveChangesAsync();

                // Тестові короткі посилання
                var url1 = new UrlMapping
                {
                    OriginalUrl = "https://example.com",
                    ShortCode = "exmpl1",
                    CreatedById = admin.Id,
                    CreatedOn = DateTime.UtcNow
                };

                var url2 = new UrlMapping
                {
                    OriginalUrl = "https://github.com",
                    ShortCode = "git1",
                    CreatedById = user1.Id,
                    CreatedOn = DateTime.UtcNow.AddMinutes(-30),
                    
                };
                var url3 = new UrlMapping
                {
                    OriginalUrl = "https://wikipedia.org",
                    ShortCode = "wik1",
                    CreatedById = user2.Id,
                    CreatedOn = DateTime.UtcNow.AddMinutes(-10),
                    
                };
                db.UrlMappings.AddRange(url1, url2, url3);

                // AboutContent
                var about = new AboutContent
                {
                    Title = "About URL Shortener",
                    Content = "This is a simple URL shortening service built with ASP.NET Core MVC and Angular integration.",
                };

                db.AboutContents.Add(about);

                await db.SaveChangesAsync();
            }
        }
    }
}

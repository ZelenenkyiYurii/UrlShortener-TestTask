using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Services;

public interface IAuthenticationService
{
    Task<User?> ValidateCredentialsAsync(string username, string password);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByUsernameAsync(string username);
}
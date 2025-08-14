
using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Data.Repositories.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string email);
    Task<bool> ExistsAsync(int id);
    Task<bool> UsernameExistsAsync(string username);
    
}
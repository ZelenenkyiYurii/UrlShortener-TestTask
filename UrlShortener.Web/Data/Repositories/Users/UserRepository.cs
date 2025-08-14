using Microsoft.EntityFrameworkCore;
using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Data.Repositories.Users;

public class UserRepository:IUserRepository
{
    private readonly UrlShortenerDbContext _context;

    public UserRepository(UrlShortenerDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u=>u.CreatedUrls)
            .FirstOrDefaultAsync(u=>u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u=>u.Username == username);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u=>u.Id == id);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u=> u.Username==username);
    }
}
using BCrypt.Net;
using UrlShortener.Web.Data.Repositories.Users;
using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Services;

public class AuthenticationService: IAuthenticationService
{
    private readonly IUserRepository _userRepository;

    public AuthenticationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User?> ValidateCredentialsAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        
        if (user == null)
            return null;
        
        if(!BCrypt.Net.BCrypt.Verify(password,user.PasswordHash))
            return null;
        
        return user;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await  _userRepository.GetByIdAsync(userId);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }
}
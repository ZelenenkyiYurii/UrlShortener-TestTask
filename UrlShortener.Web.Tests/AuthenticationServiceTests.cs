namespace UrlShortener.Web.Tests;

using Moq;
using Xunit;
using FluentAssertions;
using UrlShortener.Web.Services;
using UrlShortener.Web.Data.Repositories.Users;
using UrlShortener.Web.Models.Domain;
using System.Threading.Tasks;

public class AuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _service = new AuthenticationService(_mockUserRepo.Object);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithValidCredentials_ShouldReturnUser()
    {
        // ARRANGE
        string username = "testuser";
        string password = "password123";
      
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var expectedUser = new User { Username = username, PasswordHash = hashedPassword };

        _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(username)).ReturnsAsync(expectedUser);
        
        var result = await _service.ValidateCredentialsAsync(username, password);
        
        result.Should().NotBeNull();
        result.Should().Be(expectedUser);
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WithInvalidPassword_ShouldReturnNull()
    {
        string username = "testuser";
        string correctPassword = "password123";
        string wrongPassword = "wrongpassword";
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);
        var expectedUser = new User { Username = username, PasswordHash = hashedPassword };

        _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(username)).ReturnsAsync(expectedUser);
        
        var result = await _service.ValidateCredentialsAsync(username, wrongPassword);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ValidateCredentialsAsync_WhenUserNotFound_ShouldReturnNull()
    {
        _mockUserRepo.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
        
        var result = await _service.ValidateCredentialsAsync("nonexistentuser", "somepassword");
        
        result.Should().BeNull();
    }
}
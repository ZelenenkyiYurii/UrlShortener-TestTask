namespace UrlShortener.Web.Tests;

using Moq;
using Xunit;
using FluentAssertions;
using UrlShortener.Web.Services;
using UrlShortener.Web.Data.Repositories.AboutContents;
using UrlShortener.Web.Data.Repositories.Users;
using UrlShortener.Web.Models.Domain;
using System.Threading.Tasks;
using System;

public class AboutContentServiceTests
{
    private readonly Mock<IAboutContentRepository> _mockAboutRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly AboutContentService _service; 

    public AboutContentServiceTests()
    {
        _mockAboutRepo = new Mock<IAboutContentRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _service = new AboutContentService(_mockAboutRepo.Object, _mockUserRepo.Object);
    }
    
    [Fact]
    public async Task UpdateContentAsync_WhenUserIsAdmin_ShouldSucceedAndReturnUpdatedContent()
    {
        
        var adminUser = new User { Id = 1, Role = UserRole.Admin };
        var expectedContent = new AboutContent { Title = "New Title", Content = "New Content" };
        
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(adminUser);
        
        _mockAboutRepo.Setup(repo => repo.UpdateAsync(It.IsAny<AboutContent>())).ReturnsAsync(expectedContent);
        
        var result = await _service.UpdateContentAsync(" New Title ", " New Content ", 1);
        
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedContent);
        
        _mockAboutRepo.Verify(repo => repo.UpdateAsync(It.Is<AboutContent>(c => 
            c.Title == "New Title" && c.Content == "New Content"
        )), Times.Once);
    }
    
    [Fact]
    public async Task UpdateContentAsync_WhenUserIsNotAdmin_ShouldThrowUnauthorizedAccessException()
    {

        var regularUser = new User { Id = 2, Role = UserRole.User };
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(regularUser);
        
        Func<Task> act = async () => await _service.UpdateContentAsync("t", "c", 2);
        
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
    
    [Fact]
    public async Task UpdateContentAsync_WhenUserNotFound_ShouldThrowArgumentException()
    {

        _mockUserRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

        Func<Task> act = async () => await _service.UpdateContentAsync("t", "c", 999);
        
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("User not found. (Parameter 'userId')");
    }
}
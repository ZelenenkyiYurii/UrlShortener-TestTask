namespace UrlShortener.Web.Tests;

using Moq;
using Xunit;
using FluentAssertions;
using UrlShortener.Web.Services;
using UrlShortener.Web.Data.Repositories.UrlMappings;
using UrlShortener.Web.Data.Repositories.Users;
using UrlShortener.Web.Models.Domain;
using System.Threading.Tasks;
using System;

public class UrlShorteningServiceTests
{
    private readonly Mock<IUrlMappingRepository> _mockUrlRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly UrlShorteningService _service;

    public UrlShorteningServiceTests()
    {
        _mockUrlRepo = new Mock<IUrlMappingRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _service = new UrlShorteningService(_mockUrlRepo.Object, _mockUserRepo.Object);
    }

    #region CreateShortUrlAsync Tests

    [Fact]
    public async Task CreateShortUrlAsync_WithValidData_ShouldCreateAndReturnUrlMapping()
    {
        string originalUrl = "https://google.com";
        int userId = 1;
        var user = new User { Id = userId };
        var initialSavedUrl = new UrlMapping
            { Id = 123, OriginalUrl = originalUrl, CreatedById = userId, ShortCode = "temp" };

        _mockUrlRepo.Setup(r => r.GetByOriginalUrlAsync(originalUrl)).ReturnsAsync((UrlMapping)null);
        _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockUrlRepo.Setup(r => r.CreateAsync(It.IsAny<UrlMapping>())).ReturnsAsync(initialSavedUrl);
        _mockUrlRepo.Setup(r => r.ShortCodeExistsAsync(It.IsAny<string>())).ReturnsAsync(false);

        var result = await _service.CreateShortUrlAsync(" google.com ", userId);

        result.Should().NotBeNull();
        result.OriginalUrl.Should().Be(originalUrl);
        result.ShortCode.Should().NotBe("temp");
        result.Id.Should().Be(123);

        _mockUrlRepo.Verify(r => r.UpdateAsync(It.Is<UrlMapping>(u => u.Id == 123 && u.ShortCode != "temp")),
            Times.Once);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-url")]
    [InlineData("ftp://google.com")]
    public async Task CreateShortUrlAsync_WithInvalidUrl_ShouldThrowArgumentException(string invalidUrl)
    {
        Func<Task> act = async () => await _service.CreateShortUrlAsync(invalidUrl, 1);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task CreateShortUrlAsync_WhenUrlAlreadyExists_ShouldThrowInvalidOperationException()
    {
        var existingUrl = new UrlMapping();
        _mockUrlRepo.Setup(r => r.GetByOriginalUrlAsync(It.IsAny<string>())).ReturnsAsync(existingUrl);

        Func<Task> act = async () => await _service.CreateShortUrlAsync("https://google.com", 1);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Url already exists");
    }

    [Fact]
    public async Task CreateShortUrlAsync_WhenShortCodeGenerationFails_ShouldThrowInvalidOperationException()
    {
        _mockUrlRepo.Setup(r => r.ShortCodeExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _mockUserRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User());
        _mockUrlRepo.Setup(r => r.CreateAsync(It.IsAny<UrlMapping>())).ReturnsAsync(new UrlMapping { Id = 1 });

        Func<Task> act = () => _service.CreateShortUrlAsync("https://example.com", 1);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Failed to generate unique short code.");
    }

    #endregion

    #region DeleteUrlAsync Tests

    [Fact]
    public async Task DeleteUrlAsync_WhenUserIsAdmin_ShouldCallDeleteAsync()
    {
        int urlId = 1, userId = 1;
        bool isAdmin = true;

        await _service.DeleteUrlAsync(urlId, userId, isAdmin);

        _mockUrlRepo.Verify(r => r.DeleteAsync(urlId), Times.Once);

        _mockUrlRepo.Verify(r => r.DeleteByUserIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUrlAsync_WhenUserIsNotAdmin_ShouldCallDeleteByUserIdAsync()
    {
        int urlId = 1, userId = 1;
        bool isAdmin = false;

        await _service.DeleteUrlAsync(urlId, userId, isAdmin);

        _mockUrlRepo.Verify(r => r.DeleteByUserIdAsync(urlId, userId), Times.Once);
        _mockUrlRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
    }

    #endregion

    #region GenerateShortCode Tests

    [Fact]
    public void GenerateShortCode_WithSameId_ShouldProduceSameCode()
    {
        var code1 = _service.GenerateShortCode(123);
        var code2 = _service.GenerateShortCode(123);

        code1.Should().Be(code2);
    }

    [Fact]
    public void GenerateShortCode_WithDifferentIds_ShouldProduceDifferentCodes()
    {
        var code1 = _service.GenerateShortCode(123);
        var code2 = _service.GenerateShortCode(124);

        code1.Should().NotBe(code2);
    }

    #endregion
}
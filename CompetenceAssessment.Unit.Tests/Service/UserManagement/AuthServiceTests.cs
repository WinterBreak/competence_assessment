using System.Security.Claims;
using CompetenceAssessment.Application.UserManagement;
using CompetenceAssessment.Domain.UserManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace CompetenceAssessment.Tests.Application.UserManagement;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _service = new AuthService(
            _userRepositoryMock.Object,
            _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Success_When_Data_Is_Valid()
    {
        var httpContext = CreateHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var user = CreateUser();

        _userRepositoryMock
            .Setup(x => x.GetUserAsync(It.IsAny<UserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.ValidatePasswordAsync(user.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.LoginAsync("test@mail.com", "123", CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(user.Id, result.UserId);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Failed_When_User_Not_Found()
    {
        var httpContext = CreateHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _userRepositoryMock
            .Setup(x => x.GetUserAsync(It.IsAny<UserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var result = await _service.LoginAsync("test@mail.com", "123", CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Failed_When_Password_Is_Invalid()
    {
        var httpContext = CreateHttpContext();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        var user = CreateUser();

        _userRepositoryMock
            .Setup(x => x.GetUserAsync(It.IsAny<UserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.ValidatePasswordAsync(user.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.LoginAsync("test@mail.com", "wrong", CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task LoginAsync_Should_Return_Failed_When_HttpContext_Is_Null()
    {
        var user = CreateUser();

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns((HttpContext)null);

        _userRepositoryMock
            .Setup(x => x.GetUserAsync(It.IsAny<UserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(x => x.ValidatePasswordAsync(user.Id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _service.LoginAsync("test@mail.com", "123", CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task GetCurrentUserAsync_Should_Return_Null_When_HttpContext_Is_Null()
    {
        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns((HttpContext)null);

        var result = await _service.GetCurrentUserAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCurrentUserAsync_Should_Return_Null_When_NameIdentifier_Claim_Is_Missing()
    {
        var context = CreateHttpContext();

        context.User = new ClaimsPrincipal(new ClaimsIdentity());

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(context);

        var result = await _service.GetCurrentUserAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCurrentUserAsync_Should_Return_Null_When_NameIdentifier_Is_Invalid()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "abc")
        };

        var context = CreateHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(context);

        var result = await _service.GetCurrentUserAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCurrentUserAsync_Should_Return_User_With_Roles()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "User")
        };

        var context = CreateHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var user = CreateUser();

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(context);

        _userRepositoryMock
            .Setup(x => x.GetUserAsync(It.IsAny<UserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.GetCurrentUserAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Roles.Count);
        Assert.Contains("Admin", result.Roles);
        Assert.Contains("User", result.Roles);
    }

    [Fact]
    public async Task GetCurrentUserAsync_Should_Return_Null_When_User_Not_Found()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        };

        var context = CreateHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(context);

        _userRepositoryMock
            .Setup(x => x.GetUserAsync(It.IsAny<UserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var result = await _service.GetCurrentUserAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task LogoutAsync_Should_Not_Throw_When_HttpContext_Is_Null()
    {
        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns((HttpContext)null);

        await _service.LogoutAsync();
    }

    [Fact]
    public async Task LogoutAsync_Should_Execute_When_HttpContext_Exists()
    {
        var context = CreateHttpContext();

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(context);

        await _service.LogoutAsync();
    }

    private static HttpContext CreateHttpContext()
    {
        var services = new ServiceCollection();

        services.AddLogging();

        services
            .AddAuthentication("Cookies")
            .AddCookie("Cookies");

        var provider = services.BuildServiceProvider();

        return new DefaultHttpContext
        {
            RequestServices = provider,
            User = new ClaimsPrincipal(new ClaimsIdentity())
        };
    }

    private static User CreateUser()
    {
        return new User
        {
            Id = 1,
            Email = "test@mail.com",
            FirstName = "Test",
            LastName = "User",
            Position = "Dev",
            Department = "IT",
            DepartmentId = 1,
            PositionId = 1,
            Roles = new List<string> { "User" }
        };
    }
}
using CompetenceAssessment.Application.UserManagement;
using CompetenceAssessment.Domain.UserManagement;
using Moq;
using Xunit;

namespace CompetenceAssessment.Tests.Application.UserManagement;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _service = new UserService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetUserAsync_Should_Return_User()
    {
        var user = CreateUser();

        _repositoryMock
            .Setup(x => x.GetUserAsync(It.IsAny<UserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await _service.GetUserAsync(new UserQuery(id: 1));

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetUsersAsync_Should_Return_Users()
    {
        var users = new List<User>
        {
            CreateUser(),
            CreateUser()
        };

        _repositoryMock
            .Setup(x => x.GetUsersAsync(It.IsAny<UserQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var result = await _service.GetUsersAsync(new UserQuery());

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateUserAsync_Should_Update_User_And_Save_Changes()
    {
        var command = new UpdateUserCommand()
        {
            Id = 1,
            Email = "test@mail.com",
            FirstName = "Test",
            LastName = "User",
            DepartmentId = 1,
            PositionId = 1
        };

        var result = await _service.UpdateUserAsync(command);

        Assert.False(result.HasErrors);

        _repositoryMock.Verify(
            x => x.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _repositoryMock.Verify(
            x => x.SaveAllChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_Should_Throw_When_Command_Is_Null()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.UpdateUserAsync(null));
    }

    private static User CreateUser()
    {
        return new User
        {
            Id = 1,
            Email = "test@mail.com",
            FirstName = "Test",
            LastName = "User",
            DepartmentId = 1,
            PositionId = 1
        };
    }
}
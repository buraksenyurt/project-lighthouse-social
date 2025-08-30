using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.User;
using Moq;

namespace LighthouseSocial.Tests.Features.User;

public class GetUserByIdHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetUserByIdHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new Domain.Entities.User(userId, Guid.NewGuid(), "John Doe", "john.doe@plhsocial.com");
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(Result<Domain.Entities.User>.Ok(user));

        // Act
        var result = await _handler.HandleAsync(new GetUserByIdRequest(userId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(user.Id, result.Data!.Id);
        Assert.Equal(user.ExternalId, result.Data.SubId);
        Assert.Equal(user.Fullname, result.Data.Fullname);
        Assert.Equal(user.Email, result.Data.Email);

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(Result<Domain.Entities.User>.Fail("User not found"));

        // Act
        var result = await _handler.HandleAsync(new GetUserByIdRequest(userId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User not found", result.ErrorMessage);
        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenUserIdIsEmpty()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act
        var result = await _handler.HandleAsync(new GetUserByIdRequest(userId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("UserId is required", result.ErrorMessage);
        _userRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }
}

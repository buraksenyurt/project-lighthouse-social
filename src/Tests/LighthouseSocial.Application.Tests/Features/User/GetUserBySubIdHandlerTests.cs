using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.User;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.User;

public class GetUserBySubIdHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GetUserBySubIdHandler _handler;

    public GetUserBySubIdHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetUserBySubIdHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenUserExists()
    {
        // Arrange
        var subId = Guid.NewGuid();
        var user = new Domain.Entities.User(Guid.NewGuid(), subId, "John Doe", "john.doe@plhsocial.com");
        _userRepositoryMock.Setup(r => r.GetBySubIdAsync(subId, It.IsAny<CancellationToken>())).ReturnsAsync(Result<Domain.Entities.User>.Ok(user));

        // Act
        var result = await _handler.HandleAsync(new GetUserBySubIdRequest(subId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(user.Id, result.Data!.Id);
        Assert.Equal(user.ExternalId, result.Data.SubId);
        Assert.Equal(user.Fullname, result.Data.Fullname);
        Assert.Equal(user.Email, result.Data.Email);

        _userRepositoryMock.Verify(r => r.GetBySubIdAsync(subId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        var subId = Guid.NewGuid();
        _userRepositoryMock.Setup(r => r.GetBySubIdAsync(subId, It.IsAny<CancellationToken>())).ReturnsAsync(Result<Domain.Entities.User>.Fail("User not found"));

        // Act
        var result = await _handler.HandleAsync(new GetUserBySubIdRequest(subId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User not found", result.ErrorMessage);
        _userRepositoryMock.Verify(r => r.GetBySubIdAsync(subId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act
        var result = await _handler.HandleAsync(new GetUserBySubIdRequest(userId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("UserId is required", result.ErrorMessage);
        _userRepositoryMock.Verify(r => r.GetBySubIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

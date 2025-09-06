using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.User;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.User;

public class GetUserByEmailHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly GetUserByEmailHandler _handler;

    public GetUserByEmailHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new GetUserByEmailHandler(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenUserExists()
    {
        // Arrange
        var email = "john.doe@plhsocial.com";
        var user = new Domain.Entities.User(Guid.NewGuid(), Guid.NewGuid(), "John Doe", email);
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(Result<Domain.Entities.User>.Ok(user));

        // Act
        var result = await _handler.HandleAsync(new GetUserByEmailRequest(email), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(user.Id, result.Data!.Id);
        Assert.Equal(user.ExternalId, result.Data.SubId);
        Assert.Equal(user.Fullname, result.Data.Fullname);
        Assert.Equal(user.Email, result.Data.Email);

        _userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenEmailIsEmpty()
    {
        // Arrange
        var email = string.Empty;

        // Act
        var result = await _handler.HandleAsync(new GetUserByEmailRequest(email), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Email is required", result.ErrorMessage);
        _userRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "john.doe@plhsocial.com";
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(Result<Domain.Entities.User>.Fail("User not found"));

        // Act
        var result = await _handler.HandleAsync(new GetUserByEmailRequest(email), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("User not found", result.ErrorMessage);
        _userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
    }
}

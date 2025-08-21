using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Comment;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Tests.Features.Comment;

public class DeleteCommentHandlerTests
{
    private readonly Mock<ICommentRepository> _repositoryMock;
    private readonly DeleteCommentHandler _handler;

    public DeleteCommentHandlerTests()
    {
        _repositoryMock = new Mock<ICommentRepository>();
        _handler = new DeleteCommentHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenCommentExists()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new Domain.Entities.Comment(
            commentId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "It's a beauty lighthouse photo. Congrats!",
            Rating.FromValue(5)
        );

        _repositoryMock.Setup(r => r.GetByIdAsync(commentId))
            .ReturnsAsync(comment);

        _repositoryMock.Setup(r => r.DeleteAsync(commentId))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.HandleAsync(new DeleteCommentRequest(commentId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _repositoryMock.Verify(r => r.GetByIdAsync(commentId), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(commentId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenDeleteFails()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var comment = new Domain.Entities.Comment(
            commentId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "It's a beauty lighthouse photo. Congrats!",
            Rating.FromValue(4)
        );

        _repositoryMock.Setup(r => r.GetByIdAsync(commentId))
            .ReturnsAsync(comment);

        _repositoryMock.Setup(r => r.DeleteAsync(commentId))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.HandleAsync(new DeleteCommentRequest(commentId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Failed to delete comment", result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(commentId), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(commentId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenCommentNotFound()
    {
        // Arrange
        var commentId = Guid.NewGuid();

       _repositoryMock.Setup(r => r.GetByIdAsync(commentId))
            .ReturnsAsync((Domain.Entities.Comment?)null);

        // Act
        var result = await _handler.HandleAsync(new DeleteCommentRequest(commentId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Comment not found", result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(commentId), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}

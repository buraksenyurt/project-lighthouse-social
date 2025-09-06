using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Comment;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.Comment;

public class GetCommentsByPhotoHandlerTests
{
    private readonly Mock<ICommentRepository> _repositoryMock;
    private readonly GetCommentsByPhotoHandler _handler;

    public GetCommentsByPhotoHandlerTests()
    {
        _repositoryMock = new Mock<ICommentRepository>();
        _handler = new GetCommentsByPhotoHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnComments_WhenPhotoIdIsValid()
    {
        // Arrange
        var photoId = Guid.NewGuid();
        var comments = new List<Domain.Entities.Comment>
        {
            new(Guid.NewGuid(),Guid.NewGuid(), photoId, "Nice shot!", Rating.FromValue(7)),
            new(Guid.NewGuid(),Guid.NewGuid(), photoId, "Love this lighthouse!", Rating.FromValue(8))
        };

        _repositoryMock
            .Setup(repo => repo.GetByPhotoIdAsync(photoId))
            .Returns(Task.FromResult(Result<IEnumerable<Domain.Entities.Comment>>.Ok(comments)));

        // Act
        var result = await _handler.HandleAsync(new GetCommentsByPhotoRequest(photoId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(2, result.Data.Count());
        Assert.Equal(photoId, result.Data.First().PhotoId);

        _repositoryMock.Verify(repo => repo.GetByPhotoIdAsync(photoId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoCommentsExist()
    {
        // Arrange
        var photoId = Guid.NewGuid();
        var emptyList = new List<Domain.Entities.Comment>();

        _repositoryMock
            .Setup(repo => repo.GetByPhotoIdAsync(photoId))
            .Returns(Task.FromResult(Result<IEnumerable<Domain.Entities.Comment>>.Ok(emptyList)));

        // Act
        var result = await _handler.HandleAsync(new GetCommentsByPhotoRequest(photoId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Empty(result.Data);

        _repositoryMock.Verify(repo => repo.GetByPhotoIdAsync(photoId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenRepositoryFails()
    {
        // Arrange
        var photoId = Guid.NewGuid();

        _repositoryMock
            .Setup(repo => repo.GetByPhotoIdAsync(photoId))
            .Returns(Task.FromResult(Result<IEnumerable<Domain.Entities.Comment>>.Fail("Database error")));

        // Act
        var result = await _handler.HandleAsync(new GetCommentsByPhotoRequest(photoId), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal("Database error", result.ErrorMessage);

        _repositoryMock.Verify(repo => repo.GetByPhotoIdAsync(photoId), Times.Once);
    }
}

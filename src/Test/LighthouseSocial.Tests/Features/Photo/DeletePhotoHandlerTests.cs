using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Photo;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Tests.Features.Photo;

public class DeletePhotoHandlerTests
{
    private readonly Mock<IPhotoStorageService> _storageServiceMock;
    private readonly Mock<IPhotoRepository> _repositoryMock;
    private readonly DeletePhotoHandler _handler;

    public DeletePhotoHandlerTests()
    {
        _repositoryMock = new Mock<IPhotoRepository>();
        _storageServiceMock = new Mock<IPhotoStorageService>();
        _handler = new DeletePhotoHandler(_repositoryMock.Object, _storageServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenPhotoExists()
    {
        // Arrange
        var photoId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lighthouseId = Guid.NewGuid();
        var photo = new Domain.Entities.Photo(
            photoId,
            userId,
            lighthouseId,
            "cape_espichel.jpg",
            new PhotoMetadata("50mm", "1920x1080", "Nikon", DateTime.Now)
        );

        _repositoryMock.Setup(r => r.GetByIdAsync(photoId))
            .ReturnsAsync(photo);

        _storageServiceMock.Setup(s => s.DeleteAsync(photo.Filename))
            .Returns(Task.CompletedTask);

        _repositoryMock.Setup(r => r.DeleteAsync(photoId))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.HandleAsync(new DeletePhotoRequest(photoId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _repositoryMock.Verify(r => r.GetByIdAsync(photoId), Times.Once);
        _storageServiceMock.Verify(s => s.DeleteAsync(photo.Filename), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(photoId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenRepositoryDeleteFails()
    {
        // Arrange
        var photoId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lighthouseId = Guid.NewGuid();
        var photo = new Domain.Entities.Photo(
                photoId,
                userId,
                lighthouseId,
                "cape_espichel.jpg",
                new PhotoMetadata("50mm", "1920x1080", "Nikon", DateTime.Now)
            );

        _repositoryMock.Setup(r => r.GetByIdAsync(photoId))
            .ReturnsAsync(photo);

        _storageServiceMock.Setup(s => s.DeleteAsync(photo.Filename))
            .Returns(Task.CompletedTask);

        _repositoryMock.Setup(r => r.DeleteAsync(photoId))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.HandleAsync(new DeletePhotoRequest(photoId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Failed to delete photo from repository.", result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(photoId), Times.Once);
        _storageServiceMock.Verify(s => s.DeleteAsync(photo.Filename), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(photoId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenPhotoNotFound()
    {
        // Arrange
        var photoId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(photoId))
            .ReturnsAsync((Domain.Entities.Photo?)null);

        // Act
        var result = await _handler.HandleAsync(new DeletePhotoRequest(photoId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Photo not found.", result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(photoId), Times.Once);
        _storageServiceMock.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Never);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }    
}

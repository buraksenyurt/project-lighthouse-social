using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Photo;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.Photo;

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
            .Returns(Task.FromResult(Result<Domain.Entities.Photo>.Ok(photo)));

        _storageServiceMock.Setup(s => s.DeleteAsync(photo.Filename))
            .ReturnsAsync(Result.Ok());

        _repositoryMock.Setup(r => r.DeleteAsync(photoId))
            .Returns(Task.FromResult(Result.Ok()));

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
            .Returns(Task.FromResult(Result<Domain.Entities.Photo>.Ok(photo)));

        _storageServiceMock.Setup(s => s.DeleteAsync(photo.Filename))
            .ReturnsAsync(Result.Ok());

        _repositoryMock.Setup(r => r.DeleteAsync(photoId))
            .Returns(Task.FromResult(Result.Fail(Messages.Errors.Photo.FailedToDeletePhoto)));

        // Act
        var result = await _handler.HandleAsync(new DeletePhotoRequest(photoId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(Messages.Errors.Photo.FailedToDeletePhoto, result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(photoId), Times.Once);
        _storageServiceMock.Verify(s => s.DeleteAsync(photo.Filename), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(photoId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenPhotoNotFound()
    {// Arrange
        var photoId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(photoId))
            .Returns(Task.FromResult(Result<Domain.Entities.Photo>.Fail(Messages.Errors.Photo.PhotoNotFound)));

        // Act
        var result = await _handler.HandleAsync(new DeletePhotoRequest(photoId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(Messages.Errors.Photo.PhotoNotFound, result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(photoId), Times.Once);
        _storageServiceMock.Verify(s => s.DeleteAsync(It.IsAny<string>()), Times.Never);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}

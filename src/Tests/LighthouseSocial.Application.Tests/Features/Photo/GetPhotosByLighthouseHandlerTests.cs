using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Photo;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.Photo;

public class GetPhotosByLighthouseHandlerTests
{
    private readonly Mock<IPhotoRepository> _repositoryMock;
    private readonly GetPhotosByLighthouseHandler _handler;

    public GetPhotosByLighthouseHandlerTests()
    {
        _repositoryMock = new Mock<IPhotoRepository>();
        _handler = new GetPhotosByLighthouseHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenPhotosExist()
    {
        // Arrange
        var lighthouseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var photos = new List<Domain.Entities.Photo>
        {
            new(Guid.NewGuid(), userId, lighthouseId, "Cape Varde.jpg", new PhotoMetadata("50mm", "1920x1080", "DLSR", DateTime.Now.AddDays(-1))),
            new(Guid.NewGuid(), userId, lighthouseId, "End of the World.jpg", new PhotoMetadata("35mm", "1280x720", "SLR", DateTime.Now.AddDays(-7))),
            new(Guid.NewGuid(), userId, lighthouseId, "A New Hope.jpg", new PhotoMetadata("135mm", "1920x1920", "Mirrorless", DateTime.Now.AddDays(-3)))

        };

        _repositoryMock.Setup(r => r.GetByLighthouseIdAsync(lighthouseId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result<IEnumerable<Domain.Entities.Photo>>.Ok(photos)));

        // Act
        var result = await _handler.HandleAsync(new GetPhotosByLighthouseRequest(lighthouseId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count());

        var capeVerdePhoto = result.Data.ElementAt(0);
        Assert.Equal(photos[0].Id, capeVerdePhoto.Id);
        Assert.Equal(photos[0].Filename, capeVerdePhoto.FileName);
        Assert.Equal(photos[0].LighthouseId, capeVerdePhoto.LighthouseId);
        Assert.Equal(photos[0].UserId, capeVerdePhoto.UserId);
        Assert.Equal(photos[0].Metadata.CameraModel, capeVerdePhoto.CameraType);
        Assert.Equal(photos[0].Metadata.Resolution, capeVerdePhoto.Resolution);
        Assert.Equal(photos[0].Metadata.Lens, capeVerdePhoto.Lens);

        _repositoryMock.Verify(r => r.GetByLighthouseIdAsync(lighthouseId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenNoPhotosFound()
    {
        // Arrange
        var lighthouseId = Guid.NewGuid();
        var emptyPhotos = new List<Domain.Entities.Photo>();

        _repositoryMock.Setup(r => r.GetByLighthouseIdAsync(lighthouseId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result<IEnumerable<Domain.Entities.Photo>>.Ok(emptyPhotos)));

        // Act
        var result = await _handler.HandleAsync(new GetPhotosByLighthouseRequest(lighthouseId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(Messages.Errors.Photo.NoPhotosFoundForLighthouse, result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByLighthouseIdAsync(lighthouseId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenRepositoryReturnsNull()
    {
        // Arrange
        var lighthouseId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByLighthouseIdAsync(lighthouseId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result<IEnumerable<Domain.Entities.Photo>>.Fail("Database error")));

        // Act
        var result = await _handler.HandleAsync(new GetPhotosByLighthouseRequest(lighthouseId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Database error", result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByLighthouseIdAsync(lighthouseId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

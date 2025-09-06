using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Photo;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.Photo;

public class GetPhotosByUserHandlerTests
{
    private readonly Mock<IPhotoRepository> _repositoryMock;
    private readonly GetPhotosByUserHandler _handler;

    public GetPhotosByUserHandlerTests()
    {
        _repositoryMock = new Mock<IPhotoRepository>();
        _handler = new GetPhotosByUserHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenPhotosExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var lighthouseId1 = Guid.NewGuid();
        var lighthouseId2 = Guid.NewGuid();

        var metadata1 = new PhotoMetadata("135mm", "1920x1080", "Canon EOS Mark V", DateTime.UtcNow.AddDays(-1));
        var metadata2 = new PhotoMetadata("85mm", "3840x2160", "Sony Alpha", DateTime.UtcNow.AddDays(-7));

        var photos = new List<Domain.Entities.Photo>
    {
        new(Guid.NewGuid(), userId, lighthouseId1, "cape-town.jpg", metadata1),
        new(Guid.NewGuid(), userId, lighthouseId2, "isengard.jpg", metadata2)
    };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(Result<IEnumerable<Domain.Entities.Photo>>.Ok(photos));

        // Act
        var result = await _handler.HandleAsync(new GetPhotosByUserRequest(userId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.Count());

        var photosList = result.Data.ToList();
        Assert.Equal("cape-town.jpg", photosList[0].FileName);
        Assert.Equal("isengard.jpg", photosList[1].FileName);
        Assert.Equal(userId, photosList[0].UserId);
        Assert.Equal(userId, photosList[1].UserId);
        Assert.Equal("Canon EOS Mark V", photosList[0].CameraType);
        Assert.Equal("Sony Alpha", photosList[1].CameraType);

        _repositoryMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenSinglePhotoExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var lighthouseId = Guid.NewGuid();
        var metadata = new PhotoMetadata("135mm", "1920x1080", "Canon EOS R5", DateTime.UtcNow.AddDays(-1));
        var photos = new List<Domain.Entities.Photo>
    {
        new(Guid.NewGuid(), userId, lighthouseId, "ekinlik-adasi.jpg", metadata)
    };

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(Result<IEnumerable<Domain.Entities.Photo>>.Ok(photos));

        // Act
        var result = await _handler.HandleAsync(new GetPhotosByUserRequest(userId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!);

        var photo = result.Data.First();
        Assert.Equal("ekinlik-adasi.jpg", photo.FileName);
        Assert.Equal(userId, photo.UserId);
        Assert.Equal("Canon EOS R5", photo.CameraType);

        _repositoryMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenNoPhotosFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyPhotos = Enumerable.Empty<Domain.Entities.Photo>();

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(Result<IEnumerable<Domain.Entities.Photo>>.Ok(emptyPhotos));

        // Act
        var result = await _handler.HandleAsync(new GetPhotosByUserRequest(userId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(Messages.Errors.Photo.NoPhotosFoundForUser, result.ErrorMessage);

        _repositoryMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenRepositoryFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var errorMessage = "Database connection failed";

        _repositoryMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(Result<IEnumerable<Domain.Entities.Photo>>.Fail(errorMessage));

        // Act
        var result = await _handler.HandleAsync(new GetPhotosByUserRequest(userId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(errorMessage, result.ErrorMessage);

        _repositoryMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenUserIdIsEmpty()
    {
        // Arrange
        var emptyUserId = Guid.Empty;

        _repositoryMock.Setup(r => r.GetByUserIdAsync(emptyUserId))
            .ReturnsAsync(Result<IEnumerable<Domain.Entities.Photo>>.Fail("Invalid user ID"));

        // Act
        var result = await _handler.HandleAsync(new GetPhotosByUserRequest(emptyUserId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Invalid user ID", result.ErrorMessage);

        _repositoryMock.Verify(r => r.GetByUserIdAsync(emptyUserId), Times.Once);
    }   
}
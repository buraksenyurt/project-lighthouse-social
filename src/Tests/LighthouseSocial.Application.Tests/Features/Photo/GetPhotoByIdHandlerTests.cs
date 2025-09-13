using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Photo;
using LighthouseSocial.Domain.ValueObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LighthouseSocial.Application.Tests.Features.Photo;

public class GetPhotoByIdHandlerTests
{
    private readonly Mock<IPhotoRepository> _repositoryMock;
    private readonly GetPhotoByIdHandler _handler;

    public GetPhotoByIdHandlerTests()
    {
        _repositoryMock = new Mock<IPhotoRepository>();
        _handler = new GetPhotoByIdHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenPhotoExists()
    {
        // Arrange
        var photoId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lighthouseId = Guid.NewGuid();
        var metadata = new PhotoMetadata("135mm", "1920x1080", "Canon EOS R5", DateTime.UtcNow.AddDays(-1));
        var photo = new Domain.Entities.Photo(photoId, userId, lighthouseId, "cape-espichel.jpg", metadata);

        _repositoryMock.Setup(r => r.GetByIdAsync(photoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Domain.Entities.Photo>.Ok(photo));

        // Act
        var result = await _handler.HandleAsync(new GetPhotoByIdRequest(photoId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(photoId, result.Data!.Id);
        Assert.Equal("cape-espichel.jpg", result.Data.FileName);
        Assert.Equal(userId, result.Data.UserId);
        Assert.Equal(lighthouseId, result.Data.LighthouseId);
        Assert.Equal("Canon EOS R5", result.Data.CameraType);
        Assert.Equal("135mm", result.Data.Lens);
        Assert.Equal("1920x1080", result.Data.Resolution);

        _repositoryMock.Verify(r => r.GetByIdAsync(photoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenPhotoNotFound()
    {
        // Arrange
        var photoId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(photoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Domain.Entities.Photo>.Fail(Messages.Errors.Photo.PhotoNotFound));

        // Act
        var result = await _handler.HandleAsync(new GetPhotoByIdRequest(photoId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(Messages.Errors.Photo.PhotoNotFound, result.ErrorMessage);

        _repositoryMock.Verify(r => r.GetByIdAsync(photoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenRepositoryThrowsException()
    {
        // Arrange
        var photoId = Guid.NewGuid();
        var errorMessage = "Database connection failed";

        _repositoryMock.Setup(r => r.GetByIdAsync(photoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Domain.Entities.Photo>.Fail(errorMessage));

        // Act
        var result = await _handler.HandleAsync(new GetPhotoByIdRequest(photoId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(errorMessage, result.ErrorMessage);

        _repositoryMock.Verify(r => r.GetByIdAsync(photoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenPhotoIdIsEmpty()
    {
        // Arrange
        var emptyPhotoId = Guid.Empty;

        _repositoryMock.Setup(r => r.GetByIdAsync(emptyPhotoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Domain.Entities.Photo>.Fail("Invalid photo ID"));

        // Act
        var result = await _handler.HandleAsync(new GetPhotoByIdRequest(emptyPhotoId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("Invalid photo ID", result.ErrorMessage);

        _repositoryMock.Verify(r => r.GetByIdAsync(emptyPhotoId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Features.Photo;
using Moq;

namespace LighthouseSocial.Tests.Features.Photo;

public class GetRawPhotoHandlerTests
{
    private readonly Mock<IPhotoStorageService> _photoStorageServiceMock;
    private readonly GetRawPhotoHandler _handler;

    public GetRawPhotoHandlerTests()
    {
        _photoStorageServiceMock = new Mock<IPhotoStorageService>();
        _handler = new GetRawPhotoHandler(_photoStorageServiceMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenPhotoExists()
    {
        // Arrange
        var fileName = "cape_espichel.jpg";
        var photoStream = new MemoryStream([10, 2, 3, 48, 51, 60]);

        _photoStorageServiceMock.Setup(s => s.GetAsync(fileName))
            .ReturnsAsync(photoStream);

        // Act
        var result = await _handler.HandleAsync(new GetRawPhotoRequest(fileName), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(photoStream, result.Data);
        _photoStorageServiceMock.Verify(s => s.GetAsync(fileName), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenPhotoStreamIsEmpty()
    {
        // Arrange
        var fileName = "there-is-no-lighthouse.jpg";
        var emptyStream = new MemoryStream();

        _photoStorageServiceMock.Setup(s => s.GetAsync(fileName))
            .ReturnsAsync(emptyStream);

        // Act
        var result = await _handler.HandleAsync(new GetRawPhotoRequest(fileName), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(Messages.Errors.Photo.PhotoNotFoundInStorage, result.ErrorMessage);
        _photoStorageServiceMock.Verify(s => s.GetAsync(fileName), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenPhotoNotFound()
    {
        // Arrange
        var fileName = "there-is-no-lighthouse.jpg";

        _photoStorageServiceMock.Setup(s => s.GetAsync(fileName))
            .ReturnsAsync((Stream?)null);

        // Act
        var result = await _handler.HandleAsync(new GetRawPhotoRequest(fileName), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(Messages.Errors.Photo.PhotoNotFoundInStorage, result.ErrorMessage);
        _photoStorageServiceMock.Verify(s => s.GetAsync(fileName), Times.Once);
    }
}

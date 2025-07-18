using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Photo;
using LighthouseSocial.Domain.Interfaces;
using Moq;

namespace LighthouseSocial.Tests.Features.Photo;

public class UploadPhotoHandlerTests
{
    private readonly Mock<IPhotoStorageService> _storageMock;
    private readonly Mock<IPhotoRepository> _repositoryMock;
    private readonly UploadPhotoHandler _handler;
    public UploadPhotoHandlerTests()
    {
        _storageMock = new Mock<IPhotoStorageService>();
        _repositoryMock = new Mock<IPhotoRepository>();
        _handler = new UploadPhotoHandler(_repositoryMock.Object, _storageMock.Object);
    }
    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenValidInput()
    {
        // Arrange
        var dto = new PhotoDto(Guid.Empty, "SunDownOfCapeTown.jpg", DateTime.UtcNow, "Nikon", Guid.NewGuid(), Guid.NewGuid());

        var stream = new MemoryStream([24, 42, 32]);

        _storageMock.Setup(s => s.SaveAsync(It.IsAny<Stream>(), dto.FileName))
                    .ReturnsAsync("uploads/SunDownOfCapeTown.jpg");

        // Act
        var result = await _handler.HandleAsync(dto, stream);

        // Assert
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.Data);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Photo>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldFail_WhenContentIsEmpty()
    {
        var dto = new PhotoDto(Guid.Empty, "CapeTown_01.jpg", DateTime.UtcNow, "Canon Marc 5", Guid.NewGuid(), Guid.NewGuid());

        using var emptyStream = new MemoryStream();

        var result = await _handler.HandleAsync(dto, emptyStream);

        Assert.False(result.Success);
        Assert.Equal("Photo content is empty", result.ErrorMessage);
    }
}

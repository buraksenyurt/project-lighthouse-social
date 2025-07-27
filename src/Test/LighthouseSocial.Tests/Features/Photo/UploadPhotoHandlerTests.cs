using FluentValidation;
using FluentValidation.Results;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Photo;
using LighthouseSocial.Domain.Interfaces;
using Moq;

namespace LighthouseSocial.Tests.Features.Photo;

public class UploadPhotoHandlerTests
{
    private readonly Mock<IPhotoStorageService> _storageMock;
    private readonly Mock<IPhotoRepository> _repositoryMock;
    private readonly Mock<IValidator<PhotoDto>> _validatorMock;
    private readonly UploadPhotoHandler _handler;
    public UploadPhotoHandlerTests()
    {
        _storageMock = new Mock<IPhotoStorageService>();
        _repositoryMock = new Mock<IPhotoRepository>();
        _validatorMock = new Mock<IValidator<PhotoDto>>();
        _handler = new UploadPhotoHandler(_repositoryMock.Object, _storageMock.Object, _validatorMock.Object);
    }
    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenValidInput()
    {
        // Arrange
        var dto = new PhotoDto(Guid.Empty, "SunDownOfCapeTown.jpg", DateTime.UtcNow, "DSLR", Guid.NewGuid(), Guid.NewGuid(), "16Mp", "50mm");

        var stream = new MemoryStream([24, 42, 32]);

        _storageMock.Setup(s => s.SaveAsync(It.IsAny<Stream>(), dto.FileName))
                    .ReturnsAsync("uploads/SunDownOfCapeTown.jpg");

        _validatorMock
            .Setup(v => v.Validate(It.IsAny<PhotoDto>()))
            .Returns(new ValidationResult());

        // Act
        var result = await _handler.HandleAsync(dto, stream);

        // Assert
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.Data);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Photo>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenValidationFails()
    {
        // Arrange
        var dto = new PhotoDto(Guid.Empty, string.Empty, DateTime.Now.AddDays(2), "Noname", Guid.Empty, Guid.Empty, "16Mp", "50mm");

        var validationFailures = new List<ValidationFailure>
        {
            new("FileName","Filename can not be empty."),
            new("UploadedAt","Upload date must be in the past or now."),
            new("CameraType","Camera type is not recognized"),
            new("UserId","UserId is required."),
            new("UserId","LighthouseId is required.")
        };

        _validatorMock
            .Setup(v => v.Validate(It.IsAny<PhotoDto>()))
            .Returns(new ValidationResult(validationFailures));

        // Act
        var stream = new MemoryStream([24, 42, 32]);
        var result = await _handler.HandleAsync(dto, stream);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Filename can not be empty.", result.ErrorMessage);
        Assert.Contains("Upload date must be in the past or now.", result.ErrorMessage);
        Assert.Contains("Camera type is not recognized", result.ErrorMessage);
        Assert.Contains("UserId is required.", result.ErrorMessage);
        Assert.Contains("LighthouseId is required.", result.ErrorMessage);
    }
}

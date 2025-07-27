using FluentValidation;
using FluentValidation.Results;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Countries;
using LighthouseSocial.Domain.Interfaces;
using Moq;

namespace LighthouseSocial.Tests.Features.Lighthouse;

public class CreateLighthouseHandlerTests
{
    private readonly Mock<ILighthouseRepository> _repositoryMock;
    private readonly Mock<ICountryRegistry> _registryMock;
    private readonly Mock<IValidator<LighthouseDto>> _validatorMock;
    private readonly CreateLighthouseHandler _handler;
    public CreateLighthouseHandlerTests()
    {
        _repositoryMock = new Mock<ILighthouseRepository>();
        _registryMock = new Mock<ICountryRegistry>();
        _validatorMock = new Mock<IValidator<LighthouseDto>>();
        _handler = new CreateLighthouseHandler(_repositoryMock.Object, _registryMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenInputIsValid()
    {
        // Arrange
        var dto = new LighthouseDto(Guid.Empty, "Roman Rock", 27, 34.10, 34.13);
        var country = new Country(27, "South Africa");

        _registryMock.Setup(r => r.GetByIdAsync(dto.CountryId)).ReturnsAsync(country);
        _validatorMock.Setup(v => v.Validate(It.IsAny<LighthouseDto>())).Returns(new ValidationResult());

        // Act
        var result = await _handler.HandleAsync(dto);

        // Assert
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.Data);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Lighthouse>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenValidationFails()
    {
        // Arrange
        var dto = new LighthouseDto(Guid.Empty, string.Empty, 999, 91.0, -181.0);
        var validationFailures = new List<ValidationFailure>
        {
            new("Name","Name is required"),
            new("CountryId","CountryId must be between 0 and 255"),
            new("Latitude","Latitude must be between -90 and 90"),
            new("Longitude","Longitude must be between -180 and 180")
        };

        _validatorMock
            .Setup(v => v.Validate(It.IsAny<LighthouseDto>()))
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _handler.HandleAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Name is required", result.ErrorMessage);
        Assert.Contains("CountryId must be between 0 and 255", result.ErrorMessage);
    }

    // [Fact]
    // public async Task HandleAsync_ShouldReturnFail_WhenCountryNotFound()
    // {
    //     // Arrange
    //     var dto = new LighthouseDto(Guid.Empty, "Green Point", 999, 0, 0);
    //     _registryMock.Setup(r => r.GetById(It.IsAny<int>())).Throws(new Exception("Not found"));

    //     // Act
    //     var result = await _handler.HandleAsync(dto);

    //     // Assert
    //     Assert.False(result.Success);
    //     Assert.Contains("Invalid country", result.ErrorMessage);
    // }

    // [Fact]
    // public async Task HandleAsync_ShouldReturnFail_WhenLighthouseNameIsEmpty()
    // {
    //     // Arrange
    //     var dto = new LighthouseDto(Guid.Empty, string.Empty, 27, 0, 0);
    //     var country = new Country(27, "South Africa");

    //     _registryMock.Setup(r => r.GetById(dto.CountryId)).Returns(country);

    //     // Act
    //     var result = await _handler.HandleAsync(dto);

    //     // Assert
    //     Assert.False(result.Success);
    //     Assert.Contains("Lighthouse name is required", result.ErrorMessage);
    // }
}

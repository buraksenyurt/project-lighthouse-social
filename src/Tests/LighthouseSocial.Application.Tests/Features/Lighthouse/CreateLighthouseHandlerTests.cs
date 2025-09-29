using FluentValidation;
using FluentValidation.Results;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.Entities;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.Lighthouse;

public class CreateLighthouseHandlerTests
{
    private readonly Mock<ILighthouseRepository> _repositoryMock;
    private readonly Mock<ICountryDataReader> _registryMock;
    private readonly Mock<IValidator<LighthouseUpsertDto>> _validatorMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly CreateLighthouseHandler _handler;
    public CreateLighthouseHandlerTests()
    {
        _repositoryMock = new Mock<ILighthouseRepository>();
        _registryMock = new Mock<ICountryDataReader>();
        _validatorMock = new Mock<IValidator<LighthouseUpsertDto>>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _handler = new CreateLighthouseHandler(_repositoryMock.Object, _registryMock.Object, _validatorMock.Object, _eventPublisherMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenInputIsValid()
    {
        // Arrange
        var dto = new LighthouseUpsertDto(Guid.Empty, "Roman Rock", 27, 34.10, 34.13);
        var country = new Country(27, "South Africa");

        _registryMock.Setup(r => r.GetByIdAsync(dto.CountryId, It.IsAny<CancellationToken>())).ReturnsAsync(Result<Country>.Ok(country));
        _validatorMock.Setup(v => v.Validate(It.IsAny<LighthouseUpsertDto>())).Returns(new ValidationResult());
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.Lighthouse>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Ok());

        // Act
        var result = await _handler.HandleAsync(new CreateLighthouseRequest(dto), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotEqual(Guid.Empty, result.Data);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Lighthouse>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenValidationFails()
    {
        // Arrange
        var dto = new LighthouseUpsertDto(Guid.Empty, string.Empty, 999, 91.0, -181.0);
        var validationFailures = new List<ValidationFailure>
        {
            new("Name","Name is required"),
            new("CountryId","CountryId must be between 0 and 255"),
            new("Latitude","Latitude must be between -90 and 90"),
            new("Longitude","Longitude must be between -180 and 180")
        };

        _validatorMock
            .Setup(v => v.Validate(It.IsAny<LighthouseUpsertDto>()))
            .Returns(new ValidationResult(validationFailures));

        // Act
        var result = await _handler.HandleAsync(new CreateLighthouseRequest(dto), It.IsAny<CancellationToken>());

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Name is required", result.ErrorMessage);
        Assert.Contains("CountryId must be between 0 and 255", result.ErrorMessage);
    }
}

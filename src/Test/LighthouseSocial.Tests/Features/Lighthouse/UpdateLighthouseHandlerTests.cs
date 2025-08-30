using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Tests.Features.Lighthouse;

public class UpdateLighthouseHandlerTests
{
    private readonly Mock<ILighthouseRepository> _repositoryMock;
    private readonly UpdateLighthouseHandler _handler;

    public UpdateLighthouseHandlerTests()
    {
        _repositoryMock = new Mock<ILighthouseRepository>();
        _handler = new UpdateLighthouseHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenLighthouseExists()
    {
        // Arrange
        var lighthouseId = Guid.NewGuid();
        var country = Country.Create(42, "Portugal");
        var lighthouse = new Domain.Entities.Lighthouse(
            lighthouseId,
            "Cape Verde",
            country,
            new Coordinates(40.00, -4.00)
        );
        var dto = new LighthouseDto(
            lighthouseId,
            "Cape Espicel",
            42,
            41.8781,
            -87.6298
            );
        _repositoryMock.Setup(r => r.GetByIdAsync(lighthouseId))
            .ReturnsAsync(Result<Domain.Entities.Lighthouse>.Ok(lighthouse));
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Lighthouse>()))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _handler.HandleAsync(new UpdateLighthouseRequest(lighthouseId, dto), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _repositoryMock.Verify(r => r.GetByIdAsync(lighthouseId), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Lighthouse>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenLighthouseNotFound()
    {
        // Arrange
        var lighthouseId = Guid.NewGuid();
        var dto = new LighthouseDto(
            lighthouseId,
            "Cape Espicel",
            42,
            41.8781,
            -87.6298
        );
        _repositoryMock.Setup(r => r.GetByIdAsync(lighthouseId))
            .ReturnsAsync(Result<Domain.Entities.Lighthouse>.Fail(Messages.Errors.Lighthouse.LighthouseNotFound));

        // Act
        var result = await _handler.HandleAsync(new UpdateLighthouseRequest(lighthouseId, dto), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(Messages.Errors.Lighthouse.LighthouseNotFound, result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(lighthouseId), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Lighthouse>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenUpdateFails()
    {
        // Arrange
        var lighthouseId = Guid.NewGuid();
        var country = Country.Create(42, "Portugal");
        var lighthouse = new Domain.Entities.Lighthouse(
            lighthouseId,
            "Cape Verde",
            country,
            new Coordinates(40.00, -4.00)
        );
        var dto = new LighthouseDto(
            lighthouseId,
            "Cape Espicel",
            42,
            41.8781,
            -87.6298
        );
        _repositoryMock.Setup(r => r.GetByIdAsync(lighthouseId))
            .ReturnsAsync(Result<Domain.Entities.Lighthouse>.Ok(lighthouse));
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.Lighthouse>()))
            .ReturnsAsync(Result.Fail(Messages.Errors.Lighthouse.FailedToUpdateLighthouse));

        // Act
        var result = await _handler.HandleAsync(new UpdateLighthouseRequest(lighthouseId, dto), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(Messages.Errors.Lighthouse.FailedToUpdateLighthouse, result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(lighthouseId), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Lighthouse>()), Times.Once);
    }
}

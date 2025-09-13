using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.Lighthouse;

public class DeleteLighthouseHandlerTests
{
    private readonly Mock<ILighthouseRepository> _repositoryMock;
    private readonly DeleteLighthouseHandler _handler;

    public DeleteLighthouseHandlerTests()
    {
        _repositoryMock = new Mock<ILighthouseRepository>();
        _handler = new DeleteLighthouseHandler(_repositoryMock.Object);
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

        _repositoryMock.Setup(r => r.GetByIdAsync(lighthouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Domain.Entities.Lighthouse>.Ok(lighthouse));

        _repositoryMock.Setup(r => r.DeleteAsync(lighthouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _handler.HandleAsync(new DeleteLighthouseRequest(lighthouseId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _repositoryMock.Verify(r => r.GetByIdAsync(lighthouseId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(lighthouseId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenLighthouseNotFound()
    {
        // Arrange
        var lighthouseId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(lighthouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Domain.Entities.Lighthouse>.Fail(Messages.Errors.Lighthouse.LighthouseNotFound));

        // Act
        var result = await _handler.HandleAsync(new DeleteLighthouseRequest(lighthouseId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(Messages.Errors.Lighthouse.LighthouseNotFound, result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(lighthouseId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenDeleteFails()
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

        _repositoryMock.Setup(r => r.GetByIdAsync(lighthouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Domain.Entities.Lighthouse>.Ok(lighthouse));

        _repositoryMock.Setup(r => r.DeleteAsync(lighthouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(Messages.Errors.Lighthouse.FailedToDeleteLighthouse));

        // Act
        var result = await _handler.HandleAsync(new DeleteLighthouseRequest(lighthouseId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(Messages.Errors.Lighthouse.FailedToDeleteLighthouse, result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(lighthouseId, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(lighthouseId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

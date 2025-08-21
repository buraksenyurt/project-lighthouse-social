using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Tests.Features.Lighthouse;

public class GetLighthouseByIdHandlerTests
{
    private readonly Mock<ILighthouseRepository> _repositoryMock;
    private readonly GetLighthouseByIdHandler _handler;

    public GetLighthouseByIdHandlerTests()
    {
        _repositoryMock = new Mock<ILighthouseRepository>();
        _handler = new GetLighthouseByIdHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnLighthouse_WhenExists()
    {
        // Arrange
        var lighthouseId = Guid.NewGuid();
        var country = Country.Create(42, "Portugal");
        var lighthouse = new Domain.Entities.Lighthouse(
            lighthouseId,
            "Cape Verde",
            country,
            new Coordinates(It.IsAny<double>(), It.IsAny<double>())
        );
        _repositoryMock.Setup(r => r.GetByIdAsync(lighthouseId))
            .ReturnsAsync(lighthouse);

        // Act
        var result = await _handler.HandleAsync(new GetLighthouseByIdRequest(lighthouseId), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(lighthouse.Id, result.Data.Id);
        Assert.Equal(lighthouse.Name, result.Data.Name);
        Assert.Equal(lighthouse.CountryId, result.Data.CountryId);
        _repositoryMock.Verify(r => r.GetByIdAsync(lighthouseId), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenLighthouseNotFound()
    {
        // Arrange
        var lighthouseId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(lighthouseId))
            .ReturnsAsync((Domain.Entities.Lighthouse?)null);

        // Act
        var result = await _handler.HandleAsync(new GetLighthouseByIdRequest(lighthouseId), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Lighthouse not found.", result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetByIdAsync(lighthouseId), Times.Once);
    }
}

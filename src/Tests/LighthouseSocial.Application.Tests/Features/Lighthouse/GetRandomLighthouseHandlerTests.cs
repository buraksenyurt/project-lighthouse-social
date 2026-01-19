using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.Lighthouse;

public class GetRandomLighthouseHandlerTests
{
    private readonly Mock<ILighthouseRepository> _repositoryMock;
    private readonly GetRandomLighthouseHandler _handler;

    public GetRandomLighthouseHandlerTests()
    {
        _repositoryMock = new Mock<ILighthouseRepository>();
        _handler = new GetRandomLighthouseHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnRandomLighthouse_WhenLighthousesExist()
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
        _repositoryMock.Setup(r => r.GetRandomAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Domain.Entities.Lighthouse>.Ok(lighthouse));

        // Act
        var result = await _handler.HandleAsync(new GetRandomLighthouseRequest(), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(lighthouse.Id, result.Data.Id);
        Assert.Equal(lighthouse.Name, result.Data.Name);
        Assert.Equal(lighthouse.CountryId, result.Data.CountryId);
        _repositoryMock.Verify(r => r.GetRandomAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenNoLighthousesFound()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetRandomAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Domain.Entities.Lighthouse>.Fail("No lighthouses found."));

        // Act
        var result = await _handler.HandleAsync(new GetRandomLighthouseRequest(), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("No lighthouses found.", result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetRandomAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

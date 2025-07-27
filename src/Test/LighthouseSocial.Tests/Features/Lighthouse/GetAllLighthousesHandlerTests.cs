using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Countries;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Tests.Features.Lighthouse;

public class GetAllLighthousesHandlerTests
{
    private readonly Mock<ILighthouseRepository> _repositoryMock;
    private readonly GetAllLighthousesHandler _handler;
    public GetAllLighthousesHandlerTests()
    {
        _repositoryMock = new Mock<ILighthouseRepository>();
        _handler = new GetAllLighthousesHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_WhenThereAreLighthouses()
    {
        // Arrange
        var lighthouses = new List<Domain.Entities.Lighthouse>
        {
            new(Guid.NewGuid(),"Roman Rock", new Country(27, "South Africa"), new Coordinates(34.10, 34.13)),
            new(Guid.NewGuid(),"Green Point", new Country(27, "South Africa"), new Coordinates(24.10, 22.05)),
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(lighthouses.AsEnumerable()));
        // Act
        var result = await _handler.HandleAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Data.Count());

        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenThereAreNoLighthouses()
    {
        // Arrange
        var lighthouses = new List<Domain.Entities.Lighthouse>();
        _repositoryMock.Setup(r => r.GetAllAsync()).Returns(Task.FromResult(lighthouses.AsEnumerable()));

        // Act
        var result = await _handler.HandleAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Contains("No lighthouses found", result.ErrorMessage);

        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }
}

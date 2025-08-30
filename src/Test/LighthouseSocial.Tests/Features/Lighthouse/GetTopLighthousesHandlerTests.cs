using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Tests.Features.Lighthouse;

public class GetTopLighthousesHandlerTests
{
    private readonly Mock<ILighthouseRepository> _repositoryMock;
    private readonly GetTopLighthousesHandler _handler;

    public GetTopLighthousesHandlerTests()
    {
        _repositoryMock = new Mock<ILighthouseRepository>();
        _handler = new GetTopLighthousesHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTopLighthouses_WhenLighthousesExist()
    {
        // Arrange
        var lighthousesWithStats = Result<IEnumerable<LighthouseWithStats>>.Ok(
        [
            new() { Id = Guid.NewGuid(), Name = "End of the world", PhotoCount = 7, AverageScore = 4.4 },
            new() { Id = Guid.NewGuid(), Name = "Cape Varde", PhotoCount = 8, AverageScore = 3.8 },
            new() { Id = Guid.NewGuid(), Name = "Verdan Hope", PhotoCount = 6, AverageScore = 4.1 }
        ]);
        _repositoryMock.Setup(r => r.GetTopAsync(3))
            .ReturnsAsync(lighthousesWithStats);

        // Act
        var result = await _handler.HandleAsync(new GetTopLighthousesRequest(3), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(3, result.Data.Count());

        var firstLighthouse = result.Data.First();
        var lighthouseList = lighthousesWithStats.Data?.ToList()!;
        Assert.Equal(lighthouseList[0].Id, firstLighthouse.Id);
        Assert.Equal(lighthouseList[0].Name, firstLighthouse.Name);
        Assert.Equal(lighthouseList[0].PhotoCount, firstLighthouse.PhotoCount);
        Assert.Equal(lighthouseList[0].AverageScore, firstLighthouse.AverageScore);

        _repositoryMock.Verify(r => r.GetTopAsync(3), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenNoLighthousesFound()
    {
        // Arrange
        var emptyStats = Result<IEnumerable<LighthouseWithStats>>.Ok([]);

        _repositoryMock.Setup(r => r.GetTopAsync(5))
            .ReturnsAsync(emptyStats);

        // Act
        var result = await _handler.HandleAsync(new GetTopLighthousesRequest(5), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal(Messages.Errors.Lighthouse.NoLighthousesFound, result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetTopAsync(5), Times.Once);
    }
}

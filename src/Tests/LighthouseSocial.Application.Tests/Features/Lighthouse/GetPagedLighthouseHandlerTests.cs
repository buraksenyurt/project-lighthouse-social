using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;
using Moq;

namespace LighthouseSocial.Application.Tests.Features.Lighthouse;

public class GetPagedLighthouseHandlerTests
{
    private readonly Mock<ILighthouseRepository> _repositoryMock;
    private readonly GetPagedLighthouseHandler _handler;

    public GetPagedLighthouseHandlerTests()
    {
        _repositoryMock = new Mock<ILighthouseRepository>();
        _handler = new GetPagedLighthouseHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnPagedResult_WhenLighthousesExist()
    {
        // Arrange
        var country = Country.Create(1, "Test Country");
        var lighthouses = new List<Domain.Entities.Lighthouse>
        {
            new(Guid.NewGuid(), "Cape Verde", country, new Coordinates(40.00, -4.00)),
            new(Guid.NewGuid(), "Cape Espichel", country, new Coordinates(41.00, -5.00))
        };

        var pagingDto = new PagingDto(1, 10);
        const int totalCount = 25;

        _repositoryMock.Setup(r => r.GetPagedAsync(pagingDto.Skip, pagingDto.PageSize))
            .ReturnsAsync(Result<(IEnumerable<Domain.Entities.Lighthouse> Lighthouses, int TotalCount)>.Ok((lighthouses, totalCount)));

        // Act
        var result = await _handler.HandleAsync(new GetPagedLighthouseRequest(pagingDto), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count());
        Assert.Equal(totalCount, result.Data.TotalCount);
        Assert.Equal(pagingDto.Page, result.Data.CurrentPage);
        Assert.Equal(pagingDto.PageSize, result.Data.PageSize);

        var firstLighthouse = result.Data.Items.First();
        Assert.Equal(lighthouses[0].Id, firstLighthouse.Id);
        Assert.Equal(lighthouses[0].Name, firstLighthouse.Name);
        Assert.Equal(lighthouses[0].CountryId, firstLighthouse.CountryId);

        _repositoryMock.Verify(r => r.GetPagedAsync(pagingDto.Skip, pagingDto.PageSize), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyPagedResult_WhenNoLighthousesExist()
    {
        // Arrange
        var emptyLighthouses = new List<Domain.Entities.Lighthouse>();
        var pagingDto = new PagingDto(1, 10);
        const int totalCount = 0;

        _repositoryMock.Setup(r => r.GetPagedAsync(pagingDto.Skip, pagingDto.PageSize))
            .ReturnsAsync(Result<(IEnumerable<Domain.Entities.Lighthouse> Lighthouses, int TotalCount)>.Ok((emptyLighthouses, totalCount)));

        // Act
        var result = await _handler.HandleAsync(new GetPagedLighthouseRequest(pagingDto), CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Items);
        Assert.Equal(0, result.Data.TotalCount);
        Assert.Equal(pagingDto.Page, result.Data.CurrentPage);
        Assert.Equal(pagingDto.PageSize, result.Data.PageSize);

        _repositoryMock.Verify(r => r.GetPagedAsync(pagingDto.Skip, pagingDto.PageSize), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenRepositoryFails()
    {
        // Arrange
        var pagingDto = new PagingDto(1, 10);
        var errorMessage = "Database connection failed";

        _repositoryMock.Setup(r => r.GetPagedAsync(pagingDto.Skip, pagingDto.PageSize))
            .ReturnsAsync(Result<(IEnumerable<Domain.Entities.Lighthouse> Lighthouses, int TotalCount)>.Fail(errorMessage));

        // Act
        var result = await _handler.HandleAsync(new GetPagedLighthouseRequest(pagingDto), CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(errorMessage, result.ErrorMessage);
        _repositoryMock.Verify(r => r.GetPagedAsync(pagingDto.Skip, pagingDto.PageSize), Times.Once);
    }
}

using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Infrastructure.Caching;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Data.Repositories;

public class CachedCountryDataReader(ICountryDataReader inner, ICacheService cacheService, ILogger<CachedCountryDataReader> logger)
    : ICountryDataReader
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);
    private readonly ILogger<CachedCountryDataReader> _logger = logger;

    public async Task<Result<IReadOnlyList<Country>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            const string cacheKey = "countries:all";
            var cachedResult = await cacheService.GetAsync<IReadOnlyList<CountryDto>>(cacheKey);
            if (cachedResult.Success && cachedResult.Data != null)
            {
                var converted = cachedResult.Data.Select(c => Country.Create(c.Id, c.Name)).ToList();
                return Result<IReadOnlyList<Country>>.Ok(converted);
            }

            var result = await inner.GetAllAsync();
            if (!result.Success)
            {
                return result;
            }

            var convertedResult = result.Data!.Select(c => new CountryDto { Id = c.Id, Name = c.Name }).ToList();
            await cacheService.SetAsync(cacheKey, convertedResult, CacheDuration);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting all countries from cache");
            return Result<IReadOnlyList<Country>>.Fail($"Failed to get all countries from cache: {ex.Message}");
        }
    }

    public async Task<Result<Country>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = $"country:{id}";
            var cachedResult = await cacheService.GetAsync<CountryDto>(cacheKey);
            if (cachedResult.Success && cachedResult.Data != null)
            {
                var country = Country.Create(cachedResult.Data.Id, cachedResult.Data.Name);
                return Result<Country>.Ok(country);
            }

            var result = await inner.GetByIdAsync(id);
            if (!result.Success)
            {
                return result;
            }

            var countryData = result.Data!;
            await cacheService.SetAsync(cacheKey, new CountryDto { Id = countryData.Id, Name = countryData.Name }, CacheDuration);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting country by ID from cache. CountryId: {CountryId}", id);
            return Result<Country>.Fail(ex.Message);
        }
    }
}

//todo@buraksenyurt Country ve CountryDto arasında mapper kullanılabilir mi?
internal class CountryDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

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

    public async Task<Result<IReadOnlyList<Country>>> GetAllAsync()
    {
        try
        {
            const string cacheKey = "countries:all";
            var cached = await cacheService.GetAsync<IReadOnlyList<CountryDto>>(cacheKey);
            if (cached != null)
            {
                var converted = cached.Select(c => Country.Create(c.Id, c.Name)).ToList();
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
            logger.LogError(ex, "Error retrieving all countries from cache or inner reader");
            return Result<IReadOnlyList<Country>>.Fail($"Failed to get all countries from cache: {ex.Message}");
        }
    }

    public async Task<Result<Country>> GetByIdAsync(int id)
    {
        try
        {
            var cacheKey = $"country:{id}";
            var cached = await cacheService.GetAsync<CountryDto>(cacheKey);
            if (cached != null)
            {
                var country = Country.Create(cached.Id, cached.Name);
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
            logger.LogError(ex, "Error retrieving country with Id {CountryId} from cache or inner reader", id);
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

using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Infrastructure.Caching;
using System.Linq.Expressions;

namespace LighthouseSocial.Data.Repositories;

public class CachedCountryDataReader(ICountryDataReader inner, ICacheService cacheService)
    : ICountryDataReader
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);

    public async Task<IReadOnlyList<Country>> GetAllAsync()
    {
        const string cacheKey = "countries:all";
        var cached = await cacheService.GetAsync<IReadOnlyList<CountryDto>>(cacheKey);
        if (cached != null)
        {
            var converted = cached.Select(c => Country.Create(c.Id, c.Name)).ToList();
            return converted;
        }
        var result = await inner.GetAllAsync();
        var convertedResult = result.Select(c => new CountryDto { Id = c.Id, Name = c.Name }).ToList();
        await cacheService.SetAsync(cacheKey, convertedResult, CacheDuration);
        return result;
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

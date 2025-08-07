using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Domain.Countries;
using LighthouseSocial.Infrastructure.Caching;

namespace LighthouseSocial.Data.Repositories;

public class CachedCountryDataReader(ICountryDataReader inner, ICacheService cacheService)
    : ICountryDataReader
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);

    public async Task<IReadOnlyList<Country>> GetAllAsync()
    {
        const string cacheKey = "countries:all";
        var cached = await cacheService.GetAsync<IReadOnlyList<Country>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }
        var result = await inner.GetAllAsync();
        await cacheService.SetAsync(cacheKey, result, CacheDuration);
        return result;
    }

    public async Task<Country> GetByIdAsync(int id)
    {
        var cacheKey = $"country:{id}";
        var cached = await cacheService.GetAsync<Country>(cacheKey);
        if (cached != null)
        {
            return cached;
        }
        var result = await inner.GetByIdAsync(id);
        await cacheService.SetAsync(cacheKey, result, CacheDuration);
        return result;
    }
}

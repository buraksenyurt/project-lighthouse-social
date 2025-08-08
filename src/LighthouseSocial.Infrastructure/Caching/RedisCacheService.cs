using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace LighthouseSocial.Infrastructure.Caching;

public class RedisCacheService(IDistributedCache distributedCache)
    : ICacheService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await distributedCache.GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(json);
    }

    public Task RemoveAsync<T>(string key)
    {
        return distributedCache.RemoveAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
    {
        var json = JsonSerializer.Serialize(value);
        var options = new DistributedCacheEntryOptions();
        if (absoluteExpiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = absoluteExpiration;
        }
        await distributedCache.SetStringAsync(key, json, options);
    }
}

using Microsoft.Extensions.Caching.Memory;

namespace LighthouseSocial.Infrastructure.Caching;

public class MemoryCacheService(IMemoryCache memoryCache)
    : ICacheService
{
    public Task<T?> GetAsync<T>(string key)
    {
        memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task RemoveAsync<T>(string key)
    {
        memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
    {
        var options = new MemoryCacheEntryOptions();
        if (absoluteExpiration.HasValue)
        {
            options.SetAbsoluteExpiration(absoluteExpiration.Value);
        }
        memoryCache.Set(key,value,options);
        return Task.CompletedTask;
    }
}

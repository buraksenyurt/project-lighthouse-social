using LighthouseSocial.Application.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Infrastructure.Caching;

public class MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
    : ICacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ILogger<MemoryCacheService> _logger = logger;

    public Task<Result<T?>> GetAsync<T>(string key)
    {
        try
        {
            if (_memoryCache.TryGetValue(key, out var value))
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return Task.FromResult(Result<T?>.Ok((T?)value));
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            return Task.FromResult(Result<T?>.Ok(default(T?)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting value from memory cache. Key: {Key}", key);
            return Task.FromResult(Result<T?>.Fail($"Failed to get cache value: {ex.Message}"));
        }
    }

    public Task<Result> RemoveAsync<T>(string key)
    {
        try
        {
            _memoryCache.Remove(key);
            _logger.LogDebug("Cache key removed: {Key}", key);
            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while removing value from memory cache. Key: {Key}", key);
            return Task.FromResult(Result.Fail($"Failed to remove cache value: {ex.Message}"));
        }
    }

    public Task<Result> SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
    {
        try
        {
            if (absoluteExpiration.HasValue)
            {
                _memoryCache.Set(key, value, absoluteExpiration.Value);
            }
            else
            {
                _memoryCache.Set(key, value);
            }

            _logger.LogDebug("Cache value set for key: {Key}, Expiration: {Expiration}", key, absoluteExpiration);
            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while setting value to memory cache. Key: {Key}", key);
            return Task.FromResult(Result.Fail($"Failed to set cache value: {ex.Message}"));
        }
    }
}

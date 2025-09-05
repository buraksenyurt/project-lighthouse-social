using LighthouseSocial.Application.Common;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace LighthouseSocial.Infrastructure.Caching;

public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisCacheService> logger)
    : ICacheService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    private readonly ILogger<RedisCacheService> _logger = logger;

    public async Task<Result<T?>> GetAsync<T>(string key)
    {
        try
        {
            var json = await _database.StringGetAsync(key);
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return Result<T?>.Ok(default);
            }

            var value = JsonSerializer.Deserialize<T>(json!);
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return Result<T?>.Ok(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting value from Redis cache. Key: {Key}", key);
            return Result<T?>.Fail($"Failed to get cache value: {ex.Message}");
        }
    }

    public async Task<Result> RemoveAsync<T>(string key)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogDebug("Cache key removed: {Key}", key);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while removing value from Redis cache. Key: {Key}", key);
            return Result.Fail($"Failed to remove cache value: {ex.Message}");
        }
    }

    public async Task<Result> SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, absoluteExpiration);
            _logger.LogDebug("Cache value set for key: {Key}, Expiration: {Expiration}", key, absoluteExpiration);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while setting value to Redis cache. Key: {Key}", key);
            return Result.Fail($"Failed to set cache value: {ex.Message}");
        }
    }
}

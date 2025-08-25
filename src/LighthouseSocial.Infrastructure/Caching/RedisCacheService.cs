using StackExchange.Redis;
using System.Text.Json;

namespace LighthouseSocial.Infrastructure.Caching;

public class RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    : ICacheService
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _database.StringGetAsync(key);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }
        return JsonSerializer.Deserialize<T>(json!);
    }

    public Task RemoveAsync<T>(string key)
    {
        return _database.KeyDeleteAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, json, absoluteExpiration);
    }
}

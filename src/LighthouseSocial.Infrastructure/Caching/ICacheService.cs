namespace LighthouseSocial.Infrastructure.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null);
    Task RemoveAsync<T>(string key);
}

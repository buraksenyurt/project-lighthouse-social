using LighthouseSocial.Application.Common;

namespace LighthouseSocial.Infrastructure.Caching;

public interface ICacheService
{
    Task<Result<T?>> GetAsync<T>(string key);
    Task<Result> SetAsync<T>(string key, T value, TimeSpan? absoluteExpiration = null);
    Task<Result> RemoveAsync<T>(string key);
}

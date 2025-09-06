using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Infrastructure.Caching;
using LighthouseSocial.Infrastructure.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace LighthouseSocial.Infrastructure.Configuration;

public class CachedConfigurationService
{
    private readonly ISecretManager _secretManager;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedConfigurationService> _logger;
    private readonly ConcurrentDictionary<string, CachedItem> _memoryCache = new();
    private readonly bool _useDistributedCache;
    private const string SecretPath = "ProjectLighthouseSocial-Dev";
    private const string DatabaseConnectionKey = "vault:database_connection";
    private const string MinioCredentialsKey = "vault:minio_credentials";
    private const string KeycloakSettingsKey = "vault:keycloak_settings";

    public CachedConfigurationService(
        ISecretManager secretManager,
        ICacheService cacheService,
        ILogger<CachedConfigurationService> logger)
    {
        _secretManager = secretManager;
        _cacheService = cacheService;
        _logger = logger;
        _useDistributedCache = cacheService.GetType().Name.Contains("Redis");

        _logger.LogInformation(
            "CachedConfigurationService initialized with {CacheType} cache"
            , _useDistributedCache ? "Redis (distributed)" : "Memory (in-process)");
    }

    public async Task<string> GetDatabaseConnectionStringAsync()
    {
        return await GetCachedValueAsync(DatabaseConnectionKey,
            async () =>
            {
                var connectionStringResult = await _secretManager.GetSecretAsync(SecretPath, "DbConnStr");
                if (!connectionStringResult.Success || string.IsNullOrEmpty(connectionStringResult.Data))
                {
                    _logger.LogWarning("Database connection string not found at path: {SecretPath}. Error: {Error}",
                        SecretPath, connectionStringResult.ErrorMessage);
                    throw new InvalidOperationException(Messages.Errors.SecureVault.DatabaseConnectionStringNotFound);
                }
                _logger.LogDebug("Database connection string retrieved from Vault");
                return connectionStringResult.Data;
            },
            TimeSpan.FromHours(1));
    }

    public async Task<MinioCredentials> GetMinioCredentialsAsync()
    {
        return await GetCachedValueAsync(MinioCredentialsKey,
            async () =>
            {
                var accessKeyResult = await _secretManager.GetSecretAsync(SecretPath, "MinIOAccessKey");
                var secretKeyResult = await _secretManager.GetSecretAsync(SecretPath, "MinIOSecretKey");

                if (!accessKeyResult.Success || string.IsNullOrEmpty(accessKeyResult.Data) ||
                    !secretKeyResult.Success || string.IsNullOrEmpty(secretKeyResult.Data))
                {
                    _logger.LogWarning("MinIO credentials not found at path: {SecretPath}. AccessKey: {AccessKeyError}, SecretKey: {SecretKeyError}",
                        SecretPath, accessKeyResult.ErrorMessage, secretKeyResult.ErrorMessage);
                    throw new InvalidOperationException(Messages.Errors.SecureVault.MinioCredentialsNotFound);
                }

                _logger.LogDebug("MinIO credentials retrieved from Vault");
                return new MinioCredentials(accessKeyResult.Data, secretKeyResult.Data);
            },
            TimeSpan.FromHours(1));
    }

    public async Task<KeycloakSettings> GetKeycloakSettingsAsync()
    {
        return await GetCachedValueAsync(KeycloakSettingsKey,
            async () =>
            {
                var audienceResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakAudience");
                var authorityResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakAuthority");
                var clientIdResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakClientId");
                var clientSecretResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakClientSecret");
                var realmResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakRealm");
                var clockSkewResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakClockSkew");
                var requireHttpsResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakRequireHttpsMetadata");
                var validateAudienceResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakValidateAudience");
                var validateIssuerResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakValidateIssuer");
                var validateIssuerSigningKeyResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakValidateIssuerSigningKey");
                var validateLifetimeResult = await _secretManager.GetSecretAsync(SecretPath, "KeycloakValidateLifetime");


                var keycloakSettings = new KeycloakSettings
                {
                    Audience = audienceResult.Success ? audienceResult.Data ?? string.Empty : string.Empty,
                    Authority = authorityResult.Success ? authorityResult.Data ?? string.Empty : string.Empty,
                    ClientId = clientIdResult.Success ? clientIdResult.Data ?? string.Empty : string.Empty,
                    ClientSecret = clientSecretResult.Success ? clientSecretResult.Data ?? string.Empty : string.Empty,
                    Realm = realmResult.Success ? realmResult.Data ?? string.Empty : string.Empty,
                    ClockSkew = clockSkewResult.Success && int.TryParse(clockSkewResult.Data, out var clockSkew) ? clockSkew : 5,
                    RequireHttpsMetadata = !requireHttpsResult.Success || !bool.TryParse(requireHttpsResult.Data, out var requireHttps) || requireHttps,
                    ValidateAudience = !validateAudienceResult.Success || !bool.TryParse(validateAudienceResult.Data, out var validateAudience) || validateAudience,
                    ValidateIssuer = !validateIssuerResult.Success || !bool.TryParse(validateIssuerResult.Data, out var validateIssuer) || validateIssuer,
                    ValidateIssuerSigningKey = !validateIssuerSigningKeyResult.Success || !bool.TryParse(validateIssuerSigningKeyResult.Data, out var validateIssuerSigningKey) || validateIssuerSigningKey,
                    ValidateLifetime = !validateLifetimeResult.Success || !bool.TryParse(validateLifetimeResult.Data, out var validateLifetime) || validateLifetime
                };

                if (string.IsNullOrEmpty(keycloakSettings.Audience) ||
                   string.IsNullOrEmpty(keycloakSettings.Authority) ||
                   string.IsNullOrEmpty(keycloakSettings.ClientId) ||
                   string.IsNullOrEmpty(keycloakSettings.ClientSecret) ||
                   string.IsNullOrEmpty(keycloakSettings.Realm))
                {
                    _logger.LogWarning("Keycloak settings incomplete at path: {SecretPath}. Missing: Audience={AudienceError}, Authority={AuthorityError}, ClientId={ClientIdError}, ClientSecret={ClientSecretError}, Realm={RealmError}",
                        SecretPath,
                        !audienceResult.Success ? audienceResult.ErrorMessage : "OK",
                        !authorityResult.Success ? authorityResult.ErrorMessage : "OK",
                        !clientIdResult.Success ? clientIdResult.ErrorMessage : "OK",
                        !clientSecretResult.Success ? clientSecretResult.ErrorMessage : "OK",
                        !realmResult.Success ? realmResult.ErrorMessage : "OK");

                    throw new InvalidOperationException(Messages.Errors.SecureVault.RetrievingKeycloak);
                }

                _logger.LogDebug("Keycloak settings retrieved from Vault");
                return keycloakSettings;
            },
            TimeSpan.FromMinutes(30));
    }

    private async Task<T> GetCachedValueAsync<T>(string key, Func<Task<T>> valueFactory, TimeSpan ttl)
    {
        try
        {
            if (_useDistributedCache)
            {
                var result = await GetFromDistributedCacheAsync(key, valueFactory, ttl);
                if (EqualityComparer<T>.Default.Equals(result, default))
                {
                    return default!;
                }
                return result;
            }
            else
            {
                var result = await GetFromMemoryCacheAsync(key, valueFactory, ttl);
                if (EqualityComparer<T>.Default.Equals(result, default))
                {
                    return default!;
                }
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cached value for key: {Key}", key);
            return default!;
        }
    }

    private async Task<T> GetFromDistributedCacheAsync<T>(string key, Func<Task<T>> valueFactory, TimeSpan ttl)
    {
        var cachedResult = await _cacheService.GetAsync<T>(key);

        if (cachedResult.Success && cachedResult.Data != null)
        {
            _logger.LogDebug("Redis cache hit for key: {Key}", key);
            return cachedResult.Data;
        }

        _logger.LogDebug("Redis cache miss for key: {Key}, fetching from Vault", key);
        var value = await valueFactory();
        var setResult = await _cacheService.SetAsync(key, value, ttl);

        if (!setResult.Success)
        {
            _logger.LogWarning("Failed to cache value for key: {Key}. Error: {Error}", key, setResult.ErrorMessage);
        }

        return value;
    }

    private async Task<T> GetFromMemoryCacheAsync<T>(string key, Func<Task<T>> valueFactory, TimeSpan ttl)
    {
        if (_memoryCache.TryGetValue(key, out var cached) && DateTime.UtcNow - cached.CachedAt < ttl)
        {
            _logger.LogDebug("Memory cache hit for key: {Key}", key);

            if (cached.Value is T typedValue)
            {
                return typedValue;
            }

            _logger.LogWarning(
                "Type mismatch for cached value at key: {Key}. Expected: {ExpectedType}, Actual: {ActualType}",
                key, typeof(T).Name, cached.Value?.GetType().Name ?? "null");
        }

        _logger.LogDebug("Memory cache miss for key: {Key}, fetching from Vault", key);

        var value = await valueFactory();
        _memoryCache[key] = new CachedItem(DateTime.UtcNow, value);

        return value;
    }

    //TODO@buraksenyurt Cache Invalidation Fonksiyonları da Eklenebilir mi?

    public async Task WarmUpCacheAsync()
    {
        _logger.LogInformation("Starting Vault cache warm-up using {CacheType}", _useDistributedCache ? "Redis" : "Memory");

        try
        {
            var tasks = new Task[]
            {
                GetDatabaseConnectionStringAsync(),
                GetMinioCredentialsAsync(),
                GetKeycloakSettingsAsync()
            };

            await Task.WhenAll(tasks);
            _logger.LogInformation("Vault cache warm-up completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Vault cache warm-up");
        }
    }
    public async Task InvalidateCacheAsync<T>(string key)
    {
        _memoryCache.TryRemove(key, out _);

        if (_useDistributedCache)
        {
            await _cacheService.RemoveAsync<T>(key);
        }

        _logger.LogInformation("Cache invalidated for key: {Key}", key);
    }

    private sealed record CachedItem(DateTime CachedAt, object? Value);
}

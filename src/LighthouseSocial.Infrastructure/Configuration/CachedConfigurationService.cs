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
                var connectionString = await _secretManager.GetSecretAsync(SecretPath, "DbConnStr");
                if (string.IsNullOrEmpty(connectionString))
                {
                    _logger.LogWarning("Database connection string not found at path: {SecretPath}", SecretPath);
                    throw new InvalidOperationException(Messages.Errors.SecureVault.DatabaseConnectionStringNotFound);
                }
                _logger.LogDebug("Database connection string retrieved from Vault");
                return connectionString;
            },
            TimeSpan.FromHours(1));
    }

    public async Task<(string AccessKey, string SecretKey)> GetMinioCredentialsAsync()
    {
        return await GetCachedValueAsync(MinioCredentialsKey,
            async () =>
            {
                var accessKey = await _secretManager.GetSecretAsync(SecretPath, "MinIOAccessKey");
                var secretKey = await _secretManager.GetSecretAsync(SecretPath, "MinIOSecretKey");

                if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
                {
                    _logger.LogWarning("MinIO credentials not found at path: {SecretPath}", SecretPath);
                    throw new InvalidOperationException(Messages.Errors.SecureVault.MinioCredentialsNotFound);
                }

                _logger.LogDebug("MinIO credentials retrieved from Vault");
                return (AccessKey: accessKey, SecretKey: secretKey);
            },
            TimeSpan.FromHours(1));
    }

    public async Task<KeycloakSettings> GetKeycloakSettingsAsync()
    {
        return await GetCachedValueAsync(KeycloakSettingsKey,
            async () =>
            {
                var keycloakSettings = new KeycloakSettings
                {
                    Audience = await _secretManager.GetSecretAsync(SecretPath, "KeycloakAudience") ?? string.Empty,
                    Authority = await _secretManager.GetSecretAsync(SecretPath, "KeycloakAuthority") ?? string.Empty,
                    ClientId = await _secretManager.GetSecretAsync(SecretPath, "KeycloakClientId") ?? string.Empty,
                    ClientSecret = await _secretManager.GetSecretAsync(SecretPath, "KeycloakClientSecret") ?? string.Empty,
                    Realm = await _secretManager.GetSecretAsync(SecretPath, "KeycloakRealm") ?? string.Empty,
                    ClockSkew = int.TryParse(await _secretManager.GetSecretAsync(SecretPath, "KeycloakClockSkew"), out var clockSkew) ? clockSkew : 5,
                    RequireHttpsMetadata = !bool.TryParse(await _secretManager.GetSecretAsync(SecretPath, "KeycloakRequireHttpsMetadata"), out var requireHttps) || requireHttps,
                    ValidateAudience = !bool.TryParse(await _secretManager.GetSecretAsync(SecretPath, "KeycloakValidateAudience"), out var validateAudience) || validateAudience,
                    ValidateIssuer = !bool.TryParse(await _secretManager.GetSecretAsync(SecretPath, "KeycloakValidateIssuer"), out var validateIssuer) || validateIssuer,
                    ValidateIssuerSigningKey = !bool.TryParse(await _secretManager.GetSecretAsync(SecretPath, "KeycloakValidateIssuerSigningKey"), out var validateIssuerSigningKey) || validateIssuerSigningKey,
                    ValidateLifetime = !bool.TryParse(await _secretManager.GetSecretAsync(SecretPath, "KeycloakValidateLifetime"), out var validateLifetime) || validateLifetime
                };

                if (string.IsNullOrEmpty(keycloakSettings.Audience) ||
                   string.IsNullOrEmpty(keycloakSettings.Authority) ||
                   string.IsNullOrEmpty(keycloakSettings.ClientId) ||
                   string.IsNullOrEmpty(keycloakSettings.ClientSecret) ||
                   string.IsNullOrEmpty(keycloakSettings.Realm))
                {
                    _logger.LogWarning("Keycloak settings incomplete at path: {SecretPath}", SecretPath);
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
                return await GetFromDistributedCacheAsync(key, valueFactory, ttl);
            }
            else
            {
                return await GetFromMemoryCacheAsync(key, valueFactory, ttl);
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
        var cachedValue = await _cacheService.GetAsync<T>(key);

        if (cachedValue != null)
        {
            _logger.LogDebug("Redis cache hit for key: {Key}", key);
            return cachedValue;
        }

        _logger.LogDebug("Redis cache miss for key: {Key}, fetching from Vault", key);
        var value = await valueFactory();
        await _cacheService.SetAsync(key, value, ttl);

        return value;
    }

    private async Task<T> GetFromMemoryCacheAsync<T>(string key, Func<Task<T>> valueFactory, TimeSpan ttl)
    {
        if (_memoryCache.TryGetValue(key, out var cached) && DateTime.UtcNow - cached.CachedAt < ttl)
        {
            _logger.LogDebug("Memory cache hit for key: {Key}", key);
            return (T)cached.Value!;
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

    private record CachedItem(DateTime CachedAt, object? Value);
}

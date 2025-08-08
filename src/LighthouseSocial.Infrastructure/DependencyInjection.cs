using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Infrastructure.Auditors;
using LighthouseSocial.Infrastructure.Caching;
using LighthouseSocial.Infrastructure.Configuration;
using LighthouseSocial.Infrastructure.SecretManager;
using LighthouseSocial.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace LighthouseSocial.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPhotoStorageService, PhotoStorageService>();
        services.AddScoped<ICommentAuditor, ExternalCommentAuditor>();

        // Secret Vault
        services.AddScoped<ISecretManager, VaultSecretManager>();
        services.AddScoped<VaultConfigurationService>();

        // Storage
        services.AddScoped<IMinioClient>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MinioSettings>>().Value;
            return new MinioClient()
                .WithEndpoint(settings.Endpoint)
                .WithCredentials(settings.AccessKey, settings.SecretKey)
                .WithSSL(settings.UseSSL)
                .Build();
        });

        // Caching
        var useRedis = configuration.GetValue<bool>("Caching:UseRedis");
        if (useRedis)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = configuration.GetValue<string>("Caching:InstanceName") ?? "LighthouseSocial:";
            });
            services.AddScoped<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();
        }

        return services;
    }
}

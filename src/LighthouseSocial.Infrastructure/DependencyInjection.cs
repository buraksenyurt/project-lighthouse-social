using LighthouseSocial.Application.Contracts;
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
    public static InfrastructureBuilder AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return new InfrastructureBuilder(services, configuration);
    }
}

public class InfrastructureBuilder(IServiceCollection services, IConfiguration configuration)
{
    public InfrastructureBuilder WithSecretVault()
    {
        services.AddSingleton<ISecretManager, VaultSecretManager>();
        services.AddSingleton<VaultConfigurationService>();

        return this;
    }

    public InfrastructureBuilder WithStorage()
    {
        services.AddScoped<IPhotoStorageService, PhotoStorageService>();
        services.AddScoped(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MinioSettings>>().Value;
            return new MinioClient()
                .WithEndpoint(settings.Endpoint)
                .WithCredentials(settings.AccessKey, settings.SecretKey)
                .WithSSL(settings.UseSSL)
                .Build();
        });

        return this;
    }

    public InfrastructureBuilder WithCaching(bool useRedis)
    {
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

        return this;
    }

    public InfrastructureBuilder WithCaching()
    {
        var useRedis = configuration.GetValue<bool>("Caching:UseRedis");
        return WithCaching(useRedis);
    }

    public InfrastructureBuilder WithExternals()
    {
        services.AddScoped<ICommentAuditor, ExternalCommentAuditor>();
        return this;
    }

    public IServiceCollection Build()
    {
        return services;
    }
}

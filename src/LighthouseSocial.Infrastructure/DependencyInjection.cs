using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Infrastructure.Auditors;
using LighthouseSocial.Infrastructure.Caching;
using LighthouseSocial.Infrastructure.Configuration;
using LighthouseSocial.Infrastructure.SecretManager;
using LighthouseSocial.Infrastructure.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace LighthouseSocial.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IPhotoStorageService, PhotoStorageService>();
        services.AddScoped<ICommentAuditor,ExternalCommentAuditor>();

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
        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();

        return services;
    }
}

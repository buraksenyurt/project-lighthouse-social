using Elastic.Serilog.Sinks;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Infrastructure.Auditors;
using LighthouseSocial.Infrastructure.Caching;
using LighthouseSocial.Infrastructure.Configuration;
using LighthouseSocial.Infrastructure.SecretManager;
using LighthouseSocial.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Minio;
using Serilog;

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

    public InfrastructureBuilder WithKeycloak()
    {
        services.AddKeycloakWebApiAuthentication(options =>
        {
            using var serviceProvider = services.BuildServiceProvider();
            var vaultService = serviceProvider.GetRequiredService<VaultConfigurationService>();

            if (vaultService != null)
            {
                var keycloakSettings = vaultService.GetKeycloakSettingsAsync().GetAwaiter().GetResult();
                options.Realm = keycloakSettings.Realm;
                options.Audience = keycloakSettings.Audience;
                options.Resource = keycloakSettings.ClientId;
                options.AuthServerUrl = keycloakSettings.Authority;
                options.Credentials.Secret = keycloakSettings.ClientSecret;
                options.SslRequired = keycloakSettings.RequireHttpsMetadata.ToString();
                options.VerifyTokenAudience = keycloakSettings.ValidateAudience;
                options.TokenClockSkew = TimeSpan.FromSeconds(keycloakSettings.ClockSkew);
            }
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
             {
                 policy.RequireAuthenticatedUser();
                 policy.RequireRealmRoles("webapi-user");
             });
        }).AddKeycloakAuthorization();

        return this;
    }

    public InfrastructureBuilder WithElasticsearchLogging(IHostEnvironment environment)
    {
        var elasticSearchSettings = new ElasticsearchSettings();
        configuration.GetSection("ElasticsearchSettings").Bind(elasticSearchSettings);

        var loggerConfiguration = new LoggerConfiguration()
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.Elasticsearch([new Uri(elasticSearchSettings.Uri)], options =>
           {
               options.DataStream = new("lighthouse-social-logs");
           });

        if (environment.IsDevelopment())
        {
            loggerConfiguration.MinimumLevel.Debug();
        }
        else
        {
            loggerConfiguration.MinimumLevel.Information();
        }

        Log.Logger = loggerConfiguration.CreateLogger();
        services.AddSerilog();

        return this;
    }

    public IServiceCollection Build()
    {
        return services;
    }
}

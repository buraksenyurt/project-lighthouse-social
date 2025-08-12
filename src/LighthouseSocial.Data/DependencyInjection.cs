using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Data.Repositories;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Data;

public static class DependencyInjection
{
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<ILighthouseRepository, LighthouseRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        // services.AddScoped<ICountryDataReader, CountryRepository>();
        services.AddScoped<CountryRepository>();
        services.AddScoped<ICountryDataReader>(provider =>
        {
            var repo = provider.GetRequiredService<CountryRepository>();
            var cache = provider.GetRequiredService<ICacheService>();
            return new CachedCountryDataReader(repo, cache);
        });
    }
    public static IServiceCollection AddData(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(new NpgsqlConnectionFactory(connectionString));
        AddRepositories(services);
        return services;
    }

    public static IServiceCollection AddData(this IServiceCollection services
        , Func<IServiceProvider, string> connectionStringFactory)
    {
        services.AddSingleton<IDbConnectionFactory>(provider =>
        {
            var connectionString = connectionStringFactory(provider);
            return new NpgsqlConnectionFactory(connectionString);
        });

        AddRepositories(services);
        return services;
    }

    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory>(provider =>
        {
            string connectionString = string.Empty;

            try
            {
                var vaultService = provider.GetService<LighthouseSocial.Infrastructure.Configuration.VaultConfigurationService>();
                if (vaultService != null)
                {
                    connectionString = vaultService.GetDatabaseConnectionStringAsync().GetAwaiter().GetResult();
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        var logger = provider.GetService<ILogger<IDbConnectionFactory>>();
                        logger?.LogInformation("Database connection string retrieved from Vault");
                        return new NpgsqlConnectionFactory(connectionString);
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = provider.GetService<ILogger<IDbConnectionFactory>>();
                logger?.LogWarning(ex, "Failed to get connection string from Vault, using configuration fallback");
            }

            return new NpgsqlConnectionFactory(connectionString);
        });

        AddRepositories(services);
        return services;
    }
}

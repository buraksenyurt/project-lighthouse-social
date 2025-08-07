using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Data.Repositories;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;

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
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(new NpgsqlConnectionFactory(connectionString));
        AddRepositories(services);
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services
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
}

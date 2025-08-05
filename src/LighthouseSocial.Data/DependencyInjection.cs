using LighthouseSocial.Data.Repositories;
using LighthouseSocial.Domain.Countries;
using LighthouseSocial.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LighthouseSocial.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(new NpgsqlConnectionFactory(connectionString));

        services.AddScoped<ILighthouseRepository, LighthouseRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICountryRegistry, CountryRepository>();

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

        services.AddScoped<ILighthouseRepository, LighthouseRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICountryRegistry, CountryRepository>();

        return services;
    }
}

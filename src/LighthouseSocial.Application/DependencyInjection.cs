using FluentValidation;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Application.Services;
using LighthouseSocial.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace LighthouseSocial.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILighthouseService, LighthouseService>();
        services.AddScoped<CreateLighthouseHandler>();
        services.AddScoped<GetAllLighthousesHandler>();
        services.AddScoped<IValidator<LighthouseDto>, LighthouseDtoValidator>();

        return services;
    }
}

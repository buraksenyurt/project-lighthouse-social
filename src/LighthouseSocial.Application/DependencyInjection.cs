using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Common.Pipeline.Behaviors;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Comment;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Application.Features.Photo;
using LighthouseSocial.Application.Services;
using LighthouseSocial.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace LighthouseSocial.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ILighthouseService, LighthouseService>();
        services.AddScoped<IPhotoService, PhotoService>();

        // Pipeline
        services.AddScoped<PipelineDispatcher>();

        // Handlers
        services.AddScoped<IHandler<AddCommentRequest, Result<Guid>>, AddCommentHandler>();
        services.AddScoped<IHandler<DeleteCommentRequest, Result>, DeleteCommentHandler>();
        services.AddScoped<IHandler<GetCommentsByPhotoRequest, Result<IEnumerable<CommentDto>>>, GetCommentsByPhotoHandler>();
        services.AddScoped<IHandler<CreateLighthouseRequest, Result<Guid>>, CreateLighthouseHandler>();
        services.AddScoped<IHandler<DeleteLighthouseRequest, Result>, DeleteLighthouseHandler>();
        services.AddScoped<IHandler<GetAllLighthouseRequest, Result<IEnumerable<LighthouseDto>>>, GetAllLighthousesHandler>();
        services.AddScoped<IHandler<GetLighthouseByIdRequest, Result<LighthouseDto>>, GetLighthouseByIdHandler>();
        services.AddScoped<IHandler<DeletePhotoRequest, Result>, DeletePhotoHandler>();
        services.AddScoped<IHandler<UploadPhotoRequest, Result<Guid>>, UploadPhotoHandler>();

        // Pipeline Behaviors
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));


        // Validators
        services.AddScoped<IValidator<LighthouseDto>, LighthouseDtoValidator>();
        services.AddScoped<IValidator<PhotoDto>, PhotoDtoValidator>();

        return services;
    }
}

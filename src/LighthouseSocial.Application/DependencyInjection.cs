using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Common.Pipeline.Behaviors;
using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Comment;
using LighthouseSocial.Application.Features.Lighthouse;
using LighthouseSocial.Application.Features.Photo;
using LighthouseSocial.Application.Features.User;
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
        services.AddScoped<IHandler<GetLighthouseByIdRequest, Result<LighthouseDto>>, GetLighthouseByIdHandler>();
        services.AddScoped<IHandler<DeletePhotoRequest, Result>, DeletePhotoHandler>();
        services.AddScoped<IHandler<UploadPhotoRequest, Result<Guid>>, UploadPhotoHandler>();
        services.AddScoped<IHandler<GetRawPhotoRequest, Result<Stream>>, GetRawPhotoHandler>();
        services.AddScoped<IHandler<GetPhotosByLighthouseRequest, Result<IEnumerable<PhotoDto>>>, GetPhotosByLighthouseHandler>();
        services.AddScoped<IHandler<GetTopLighthousesRequest, Result<IEnumerable<LighthouseTopDto>>>, GetTopLighthousesHandler>();
        services.AddScoped<IHandler<UpdateLighthouseRequest, Result>, UpdateLighthouseHandler>();
        services.AddScoped<IHandler<GetPagedLighthouseRequest, Result<PagedResult<LighthouseDto>>>, GetPagedLighthouseHandler>();
        services.AddScoped<IHandler<CreateUserRequest, Result<Guid>>, CreateUserHandler>();

        // Pipeline Behaviors
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));

        // Validators
        services.AddScoped<IValidator<LighthouseDto>, LighthouseDtoValidator>();
        services.AddScoped<IValidator<PhotoDto>, PhotoDtoValidator>();
        services.AddScoped<IValidator<CommentDto>, CommentDtoValidator>();
        services.AddScoped<IValidator<UserDto>, UserDtoValidator>();

        return services;
    }
}

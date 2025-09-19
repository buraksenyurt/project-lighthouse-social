using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.Events;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Photo;
public record UploadPhotoRequest(PhotoDto Photo, Stream Content);

internal class UploadPhotoHandler(
    IPhotoRepository repository,
    IPhotoStorageService storageService,
    IValidator<PhotoDto> validator,
    IEventPublisher eventPublisher)
    : IHandler<UploadPhotoRequest, Result<Guid>>
{
    private readonly IPhotoStorageService _storageService = storageService;
    private readonly IPhotoRepository _repository = repository;
    private readonly IValidator<PhotoDto> _validator = validator;
    private readonly IEventPublisher _eventPublisher = eventPublisher;

    public async Task<Result<Guid>> HandleAsync(UploadPhotoRequest request, CancellationToken cancellationToken)
    {
        var dto = request.Photo;

        var uploadRequestedEvent = new PhotoUploadRequested(
            dto.Id,
            dto.FileName,
            dto.UserId,
            dto.LighthouseId,
            dto.CameraType,
            dto.Resolution,
            dto.Lens
        );
        await _eventPublisher.PublishAsync(uploadRequestedEvent, cancellationToken);

        var validation = _validator.Validate(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));

            var failureEvent = new PhotoUploadFailed(
                dto.Id,
                dto.FileName,
                dto.UserId,
                dto.LighthouseId,
                dto.CameraType,
                dto.Resolution,
                dto.Lens,
                "Validation failed",
                errors
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(errors);
        }

        var saveResult = await _storageService.SaveAsync(request.Content, dto.FileName, cancellationToken);
        if (!saveResult.Success)
        {
            var failureEvent = new PhotoUploadFailed(
                dto.Id,
                dto.FileName,
                dto.UserId,
                dto.LighthouseId,
                dto.CameraType,
                dto.Resolution,
                dto.Lens,
                "File storage failed",
                saveResult.ErrorMessage
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(saveResult.ErrorMessage!);
        }

        var metadata = new PhotoMetadata(
            dto.Lens,
            dto.Resolution,
            dto.CameraType,
            dto.UploadedAt
        );

        var photo = new Domain.Entities.Photo(dto.Id, dto.UserId, dto.LighthouseId, saveResult.Data!, metadata);

        var result = await _repository.AddAsync(photo, cancellationToken);
        if (!result.Success)
        {
            var failureEvent = new PhotoUploadFailed(
                dto.Id,
                dto.FileName,
                dto.UserId,
                dto.LighthouseId,
                dto.CameraType,
                dto.Resolution,
                dto.Lens,
                "Database save failed",
                result.ErrorMessage
            );
            await _eventPublisher.PublishAsync(failureEvent, cancellationToken);

            return Result<Guid>.Fail(result.ErrorMessage!);
        }

        var successEvent = new PhotoUploaded(
            photo.Id,
            photo.Filename,
            photo.UserId,
            photo.LighthouseId,
            photo.Metadata.CameraModel,
            photo.Metadata.Resolution,
            photo.Metadata.Lens,
            photo.UploadDate
        );
        await _eventPublisher.PublishAsync(successEvent, cancellationToken);

        return Result<Guid>.Ok(photo.Id);
    }
}

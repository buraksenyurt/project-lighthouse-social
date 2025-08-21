using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Photo;
internal record UploadPhotoRequest(PhotoDto Photo, Stream Content);

internal class UploadPhotoHandler(
    IPhotoRepository repository,
    IPhotoStorageService storageService,
    IValidator<PhotoDto> validator)
    : IHandler<UploadPhotoRequest, Result<Guid>>
{
    private readonly IPhotoStorageService _storageService = storageService;
    private readonly IPhotoRepository _repository = repository;
    private readonly IValidator<PhotoDto> _validator = validator;

    public async Task<Result<Guid>> HandleAsync(UploadPhotoRequest request, CancellationToken cancellationToken)
    {
        var dto = request.Photo;
        var validation = _validator.Validate(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Result<Guid>.Fail(errors);
        }

        var savedPath = await _storageService.SaveAsync(request.Content, dto.FileName);

        var metadata = new PhotoMetadata(
            dto.Lens,
            dto.Resolution,
            dto.CameraType,
            dto.UploadedAt
        );

        var photo = new Domain.Entities.Photo(dto.Id, dto.UserId, dto.LighthouseId, savedPath, metadata);

        var result = await _repository.AddAsync(photo);
        if (!result)
        {
            return Result<Guid>.Fail(Messages.Errors.Photo.FailedToAddPhoto);
        }

        return Result<Guid>.Ok(photo.Id);
    }
}

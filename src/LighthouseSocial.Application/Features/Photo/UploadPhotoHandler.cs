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

        var saveResult = await _storageService.SaveAsync(request.Content, dto.FileName);
        if (!saveResult.Success)
        {
            return Result<Guid>.Fail(saveResult.ErrorMessage!);
        }

        var metadata = new PhotoMetadata(
            dto.Lens,
            dto.Resolution,
            dto.CameraType,
            dto.UploadedAt
        );

        var photo = new Domain.Entities.Photo(dto.Id, dto.UserId, dto.LighthouseId, saveResult.Data!, metadata);

        var result = await _repository.AddAsync(photo);
        if (!result.Success)
        {
            return Result<Guid>.Fail(result.ErrorMessage!);
        }

        return Result<Guid>.Ok(photo.Id);
    }
}

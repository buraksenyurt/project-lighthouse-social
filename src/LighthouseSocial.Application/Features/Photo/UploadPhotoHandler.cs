using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Photo.Models;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Photo;

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

        await _repository.AddAsync(photo);

        return Result<Guid>.Ok(photo.Id);
    }
}

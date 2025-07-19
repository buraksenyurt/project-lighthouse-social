using FluentValidation;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Photo;

public class UploadPhotoHandler(IPhotoRepository repository, IPhotoStorageService storageService, IValidator<PhotoDto> validator)
{
    private readonly IPhotoStorageService _storageService = storageService;
    private readonly IPhotoRepository _repository = repository;
    private readonly IValidator<PhotoDto> _validator = validator;

    public async Task<Result<Guid>> HandleAsync(PhotoDto dto, Stream content)
    {
        // if (content == null || content.Length == 0)
        //     return Result<Guid>.Fail("Photo content is empty");

        var validation= _validator.Validate(dto);
        if (!validation.IsValid)
        {
            var errors = string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));
            return Result<Guid>.Fail(errors);
        }

        var savedPath = await _storageService.SaveAsync(content, dto.FileName);

        var metadata = new PhotoMetada(
            "N/A",
            "Unknown",
            dto.CameraType,
            dto.UploadedAt
        );

        var photo = new Domain.Entities.Photo(dto.UserId, dto.LighthouseId, savedPath, metadata);

        await _repository.AddAsync(photo);

        return Result<Guid>.Ok(photo.Id);
    }
}

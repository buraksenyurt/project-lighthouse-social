using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Application.Features.Photo;

public class UploadPhotoHandler(IPhotoRepository repository, IPhotoStorageService storageService)
{
    private readonly IPhotoStorageService _storageService = storageService;
    private readonly IPhotoRepository _repository = repository;

    public async Task<Result<Guid>> HandleAsync(PhotoDto dto, Stream content)
    {
        if (content == null || content.Length == 0)
            return Result<Guid>.Fail("Photo content is empty");

        var savedPath = await _storageService.SaveAsync(content, dto.FileName);

        var metadata = new PhotoMetada(
            "N/A",
            "Unknown",
            dto.CameraModel,
            dto.UploadedAt
        );

        var photo = new Domain.Entities.Photo(dto.UserId, dto.LighthouseId, savedPath, metadata);

        await _repository.AddAsync(photo);

        return Result<Guid>.Ok(photo.Id);
    }
}

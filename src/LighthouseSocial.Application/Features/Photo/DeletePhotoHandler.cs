using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Photo;

public class DeletePhotoHandler(
    IPhotoRepository repository,
    IPhotoStorageService storage)
{
    private readonly IPhotoRepository _repository = repository;
    private readonly IPhotoStorageService _storage = storage;

    public async Task<Result> HandleAsync(Guid photoId)
    {
        var photo = await _repository.GetByIdAsync(photoId);
        if (photo == null)
            return Result.Fail("Photo not found.");

        await _storage.DeleteAsync(photo.Filename);
        await _repository.DeleteAsync(photoId);

        return Result.Ok();
    }
}

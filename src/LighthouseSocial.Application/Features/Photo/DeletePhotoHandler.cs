using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Features.Photo.Models;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Application.Features.Photo;
internal class DeletePhotoHandler(
    IPhotoRepository repository,
    IPhotoStorageService storage)
    : IHandler<DeletePhotoRequest, Result>
{
    private readonly IPhotoRepository _repository = repository;
    private readonly IPhotoStorageService _storage = storage;

    public async Task<Result> HandleAsync(DeletePhotoRequest request, CancellationToken cancellationToken)
    {
        var photo = await _repository.GetByIdAsync(request.PhotoId);
        if (photo == null)
            return Result.Fail("Photo not found.");

        await _storage.DeleteAsync(photo.Filename);
        await _repository.DeleteAsync(request.PhotoId);

        return Result.Ok();
    }
}

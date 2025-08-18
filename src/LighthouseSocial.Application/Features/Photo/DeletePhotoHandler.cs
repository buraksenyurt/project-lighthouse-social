using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Application.Contracts.Repositories;

namespace LighthouseSocial.Application.Features.Photo;
internal record DeletePhotoRequest(Guid PhotoId);

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
        var result = await _repository.DeleteAsync(request.PhotoId);
        if (!result)
        {
            return Result.Fail("Failed to delete photo from repository.");
        }

        return Result.Ok();
    }
}

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
        var photoResult = await _repository.GetByIdAsync(request.PhotoId, cancellationToken);
        if (!photoResult.Success)
            return Result.Fail(photoResult.ErrorMessage!);

        var photo = photoResult.Data!;
        await _storage.DeleteAsync(photo.Filename, cancellationToken);

        var deleteResult = await _repository.DeleteAsync(request.PhotoId, cancellationToken);
        if (!deleteResult.Success)
        {
            return Result.Fail(deleteResult.ErrorMessage!);
        }

        return Result.Ok();
    }
}

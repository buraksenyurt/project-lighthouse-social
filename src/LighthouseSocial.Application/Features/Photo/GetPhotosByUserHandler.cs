using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Photo;

internal record GetPhotosByUserRequest(Guid UserId);

internal class GetPhotosByUserHandler(IPhotoRepository photoRepository)
    : IHandler<GetPhotosByUserRequest, Result<IEnumerable<PhotoDto>>>
{
    public async Task<Result<IEnumerable<PhotoDto>>> HandleAsync(GetPhotosByUserRequest request, CancellationToken cancellationToken)
    {
        var photosResult = await photoRepository.GetByUserIdAsync(request.UserId);

        if (!photosResult.Success)
            return Result<IEnumerable<PhotoDto>>.Fail(photosResult.ErrorMessage!);

        var photos = photosResult.Data!;
        if (!photos.Any())
            return Result<IEnumerable<PhotoDto>>.Fail(Messages.Errors.Photo.NoPhotosFoundForUser);

        var photoDtos = photos.Select(photo => new PhotoDto
        (
            photo.Id,
            photo.Filename,
            photo.UploadDate,
            photo.Metadata.CameraModel,
            photo.UserId,
            photo.LighthouseId,
            photo.Metadata.Resolution,
            photo.Metadata.Lens
        )).ToList();

        return Result<IEnumerable<PhotoDto>>.Ok(photoDtos);
    }
}

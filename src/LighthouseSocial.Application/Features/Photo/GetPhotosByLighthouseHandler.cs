using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Photo;

internal record GetPhotosByLighthouseRequest(Guid LighthouseId);

internal class GetPhotosByLighthouseHandler(IPhotoRepository photoRepository)
    : IHandler<GetPhotosByLighthouseRequest, Result<IEnumerable<PhotoDto>>>
{
    public async Task<Result<IEnumerable<PhotoDto>>> HandleAsync(GetPhotosByLighthouseRequest request, CancellationToken cancellationToken)
    {
        var photos = await photoRepository.GetByLighthouseIdAsync(request.LighthouseId);
        if (photos is null || !photos.Any())
        {
            return Result<IEnumerable<PhotoDto>>.Fail("No photos found for the specified lighthouse.");
        }
        var photoDtos = photos.Select(
            p => new PhotoDto(
                p.Id,
                p.Filename,
                p.UploadDate,
                p.Metadata.CameraModel,
                p.UserId,
                p.LighthouseId,
                p.Metadata.Resolution,
                p.Metadata.Lens)
        ).ToList();
        return Result<IEnumerable<PhotoDto>>.Ok(photoDtos);
    }
}

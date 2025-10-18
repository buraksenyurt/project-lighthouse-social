using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.Pipeline;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Application.Features.Photo;

internal record GetPhotoByIdRequest(Guid PhotoId);

internal class GetPhotoByIdHandler(IPhotoRepository photoRepository)
    : IHandler<GetPhotoByIdRequest, Result<PhotoDto>>
{
    public async Task<Result<PhotoDto>> HandleAsync(GetPhotoByIdRequest request, CancellationToken cancellationToken)
    {
        var photoResult = await photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);

        if (!photoResult.Success)
            return Result<PhotoDto>.Fail(photoResult.ErrorMessage!);

        var photo = photoResult.Data!;
        var dto = new PhotoDto
        (
            photo.Id,
            photo.Filename,
            photo.UploadDate,
            photo.Metadata.CameraModel,
            photo.UserId,
            photo.LighthouseId,
            photo.Metadata.Resolution,
            photo.Metadata.Lens,
            photo.IsPrimary
        );

        return Result<PhotoDto>.Ok(dto);
    }
}

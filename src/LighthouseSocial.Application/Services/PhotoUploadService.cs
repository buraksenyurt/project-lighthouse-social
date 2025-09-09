using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.ExternalServices;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Photo;
using LighthouseSocial.Application.Features.Photo.Saga;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Application.Services;

public class PhotoUploadService(PhotoUploadSaga photoUploadSaga, ILogger<PhotoUploadService> logger)
    : IPhotoUploadService
{
    public async Task<Result<PhotoDto>> UploadAsync(PhotoDto dto, Stream fileContent)
    {
        try
        {
            logger.LogInformation("Starting photo upload for PhotoId {PhotoId}", dto.Id);
            var result = await photoUploadSaga.ExecuteAsync(new UploadPhotoRequest(dto, fileContent), CancellationToken.None);

            if (!result.Success)
            {
                logger.LogError("Photo upload failed for PhotoId {PhotoId}, Error: {Error}", dto.Id, result.ErrorMessage);
                return Result<PhotoDto>.Fail(result.ErrorMessage!);
            }

            return result;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during photo upload for PhotoId {PhotoId}", dto.Id);
            return Result<PhotoDto>.Fail("An unexpected error occurred during photo upload.");
        }
    }
}

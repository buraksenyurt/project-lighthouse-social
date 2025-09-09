using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.DistribtedTransaction.Saga;
using LighthouseSocial.Application.Contracts.Repositories;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Application.Features.Photo.Saga.Steps;

public class MetadataSaveStep(IPhotoRepository photoRepository, ILogger<MetadataSaveStep> logger)
    : ISagaStep<PhotoUploadSagaData>
{
    public async Task<Result<PhotoUploadSagaData>> ExecuteAsync(PhotoUploadSagaData data, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Starting metadata save for PhotoId: {PhotoId}", data.PhotoId);

            if (data.PhotoEntity == null)
            {
                return Result<PhotoUploadSagaData>.Fail("Photo entity is null");
            }

            data.PhotoEntity.SetFileName(data.FileName);

            var result = await photoRepository.AddAsync(data.PhotoEntity);
            if(!result.Success)
            {
                logger.LogError("Metadata save failed for PhotoId: {PhotoId}, Error: {Error}", data.PhotoId, result.ErrorMessage);
                return Result<PhotoUploadSagaData>.Fail($"Metadata save failed: {result.ErrorMessage}");
            }

            data.IsMetadataSaved = true;

            logger.LogInformation("Metadata save completed for PhotoId: {PhotoId}", data.PhotoId);

            return Result<PhotoUploadSagaData>.Ok(data);

        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Metadata save failed for PhotoId: {PhotoId}", data.PhotoId);
            data.LastException = ex;
            return Result<PhotoUploadSagaData>.Fail($"Metadata save failed: {ex.Message}");
        }
    }
    public async Task CompensateAsync(PhotoUploadSagaData data, CancellationToken cancellationToken = default)
    {
        if (!data.IsMetadataSaved)
        {
            logger.LogInformation("No metadata to delete for PhotoId: {PhotoId}", data.PhotoId);
            return;
        }

        try
        {
            logger.LogInformation("Starting metadata deletion for PhotoId: {PhotoId}", data.PhotoId);

            var result = await photoRepository.DeleteAsync(data.PhotoId);
            if(!result.Success)
            {
                logger.LogError("Metadata deletion failed for PhotoId: {PhotoId}, Error: {Error}", data.PhotoId, result.ErrorMessage);
                return;
            }
            else
            {
                data.IsMetadataSaved = false;
                logger.LogInformation("Metadata deletion completed for PhotoId: {PhotoId}", data.PhotoId);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Metadata deletion failed for PhotoId: {PhotoId}", data.PhotoId);
        }
    }
}

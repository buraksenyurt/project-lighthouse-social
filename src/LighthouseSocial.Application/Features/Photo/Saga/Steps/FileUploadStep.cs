using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.DistribtedTransaction.Saga;
using LighthouseSocial.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Application.Features.Photo.Saga.Steps;

public class FileUploadStep(IPhotoStorageService photoStorageService, ILogger<FileUploadStep> logger)
    : ISagaStep<PhotoUploadSagaData>
{
    public async Task<Result<PhotoUploadSagaData>> ExecuteAsync(PhotoUploadSagaData data, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Starting file upload for PhotoId: {PhotoId}", data.PhotoId);

            data.FileStream.Position = 0;

            var fileNameResult = await photoStorageService.SaveAsync(data.FileStream, data.FileName);
            if(!fileNameResult.Success)
            {
                logger.LogError("File upload failed for PhotoId: {PhotoId}, Error: {Error}", data.PhotoId, fileNameResult.ErrorMessage);
                return Result<PhotoUploadSagaData>.Fail($"File upload failed: {fileNameResult.ErrorMessage}");
            }

            data.FileName = fileNameResult.Data!;
            data.IsFileUploaded = true;

            logger.LogInformation("File upload completed for PhotoId: {PhotoId}, FileName: {FileName}", data.PhotoId, data.FileName);
            return Result<PhotoUploadSagaData>.Ok(data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "File upload failed for PhotoId: {PhotoId}", data.PhotoId);
            data.LastException = ex;
            return Result<PhotoUploadSagaData>.Fail($"File upload failed:{ex.Message}");
        }
    }

    public async Task CompensateAsync(PhotoUploadSagaData data, CancellationToken cancellationToken = default)
    {
        if (!data.IsFileUploaded || string.IsNullOrEmpty(data.FileName))
        {
            logger.LogInformation("No file to delete for PhotoId: {PhotoId}", data.PhotoId);
            return;
        }

        try
        {
            logger.LogInformation("Starting file deletion for PhotoId: {PhotoId}, FileName: {FileName}", data.PhotoId, data.FileName);

            await photoStorageService.DeleteAsync(data.FileName);
            data.IsFileUploaded = false;

            logger.LogInformation("File deletion completed for PhotoId: {PhotoId}, FileName: {FileName}", data.PhotoId, data.FileName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "File deletion failed for PhotoId: {PhotoId}, FileName: {FileName}", data.PhotoId, data.FileName);
        }
    }
}

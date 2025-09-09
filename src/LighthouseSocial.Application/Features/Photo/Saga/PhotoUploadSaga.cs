using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.DistribtedTransaction.Saga;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Photo.Saga.Steps;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Application.Features.Photo.Saga;

public class PhotoUploadSaga(ILogger<PhotoUploadSaga> logger, FileUploadStep fileUploadStep, MetadataSaveStep metadataSaveStep) : ISaga<UploadPhotoRequest, Result<PhotoDto>>
{
    async Task<Result<PhotoDto>> ISaga<UploadPhotoRequest, Result<PhotoDto>>.ExecuteAsync(UploadPhotoRequest request, CancellationToken cancellationToken)
    {
        var sagaId = Guid.NewGuid();
        logger.LogInformation("Starting PhotoUploadSaga {SagaId} for PhotoId {PhotoId}", sagaId, request.Photo.Id);

        var sagaData = new PhotoUploadSagaData
        {
            PhotoId = request.Photo.Id,
            FileStream = request.Content,
            PhotoEntity = new Domain.Entities.Photo(
                request.Photo.Id,
                request.Photo.UserId,
                request.Photo.LighthouseId,
                request.Photo.FileName,
                new Domain.ValueObjects.PhotoMetadata(
                    request.Photo.Lens,
                    request.Photo.Resolution,
                    request.Photo.CameraType,
                    request.Photo.UploadedAt
                )
            )
        };

        var executedStpes = new List<ISagaStep<PhotoUploadSagaData>>();

        try
        {
            var fileUploadResult = await fileUploadStep.ExecuteAsync(sagaData, cancellationToken);
            if (!fileUploadResult.Success)
            {
                logger.LogError("File upload step failed in PhotoUploadSaga {SagaId} for PhotoId {PhotoId}, Error: {Error}", sagaId, request.Photo.Id, fileUploadResult.ErrorMessage);
                return Result<PhotoDto>.Fail(Messages.Errors.Photo.FailedToAddPhoto);
            }
            executedStpes.Add(fileUploadStep);

            var metadataSaveResult = await metadataSaveStep.ExecuteAsync(sagaData, cancellationToken);
            if (!metadataSaveResult.Success)
            {
                logger.LogError("Metadata save step failed in PhotoUploadSaga {SagaId} for PhotoId {PhotoId}, Error: {Error}", sagaId, request.Photo.Id, metadataSaveResult.ErrorMessage);
                await CompensateAsync(executedStpes, sagaData, cancellationToken);
                return Result<PhotoDto>.Fail(Messages.Errors.Photo.FailedToAddPhoto);
            }


            var photoDto = new PhotoDto(
                sagaData.PhotoEntity!.Id,
                sagaData.PhotoEntity.Filename,
                sagaData.PhotoEntity.Metadata.TakenAt,
                sagaData.PhotoEntity.Metadata.CameraModel,
                sagaData.PhotoEntity.UserId,
                sagaData.PhotoEntity.LighthouseId,
                sagaData.PhotoEntity.Metadata.Resolution,
                sagaData.PhotoEntity.Metadata.Lens
            );

            logger.LogInformation("PhotoUploadSaga {SagaId} completed successfully for PhotoId {PhotoId}", sagaId, request.Photo.Id);

            return Result<PhotoDto>.Ok(photoDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "PhotoUploadSaga {SagaId} failed for PhotoId {PhotoId}", sagaId, request.Photo.Id);
            await CompensateAsync(executedStpes, sagaData, cancellationToken);
            return Result<PhotoDto>.Fail(Messages.Errors.Photo.FailedToAddPhoto);
        }
    }

    private async Task CompensateAsync(List<ISagaStep<PhotoUploadSagaData>> executedSteps, PhotoUploadSagaData data, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting compensation for PhotoUploadSaga for PhotoId {PhotoId}", data.PhotoId);

        for (int i = executedSteps.Count - 1; i >= 0; i--)
        {
            var step = executedSteps[i];
            try
            {
                await step.CompensateAsync(data, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Compensation failed for step {Step} in PhotoUploadSaga for PhotoId {PhotoId}", step.GetType().Name, data.PhotoId);
            }
        }

        logger.LogInformation("Compensation completed for PhotoUploadSaga for PhotoId {PhotoId}", data.PhotoId);
    }
}

using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Common.DistribtedTransaction.Saga;
using LighthouseSocial.Application.Dtos;
using LighthouseSocial.Application.Features.Photo.Saga.Steps;
using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.Events.Photo;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Application.Features.Photo.Saga;

public class PhotoUploadSaga(ILogger<PhotoUploadSaga> logger, FileUploadStep fileUploadStep, MetadataSaveStep metadataSaveStep, IEventPublisher eventPublisher) : ISaga<UploadPhotoRequest, Result<PhotoDto>>
{
    public async Task<Result<PhotoDto>> ExecuteAsync(UploadPhotoRequest request, CancellationToken cancellationToken = default)
    {
        var sagaId = Guid.NewGuid();
        logger.LogInformation("Starting PhotoUploadSaga {SagaId} for PhotoId {PhotoId}", sagaId, request.Photo.Id);

        var sagaStartedEvent = new PhotoUploadSagaStarted(
            request.Photo.Id,
            sagaId,
            request.Photo.FileName,
            request.Photo.UserId,
            request.Photo.LighthouseId,
            request.Photo.CameraType,
            request.Photo.Resolution,
            request.Photo.Lens
        );
        await eventPublisher.PublishAsync(sagaStartedEvent, cancellationToken);

        var sagaData = new PhotoUploadSagaData
        {
            PhotoId = request.Photo.Id,
            FileStream = request.Content,
            FileName = request.Photo.FileName,
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
                ),
                request.Photo.IsPrimary
            )
        };

        var executedStpes = new List<ISagaStep<PhotoUploadSagaData>>();

        try
        {
            var fileUploadResult = await fileUploadStep.ExecuteAsync(sagaData, cancellationToken);
            if (!fileUploadResult.Success)
            {
                logger.LogError("File upload step failed in PhotoUploadSaga {SagaId} for PhotoId {PhotoId}, Error: {Error}", sagaId, request.Photo.Id, fileUploadResult.ErrorMessage);
                
                var fileUploadFailureEvent = new PhotoUploadSagaFailed(
                    request.Photo.Id,
                    sagaId,
                    request.Photo.FileName,
                    request.Photo.UserId,
                    request.Photo.LighthouseId,
                    request.Photo.CameraType,
                    request.Photo.Resolution,
                    request.Photo.Lens,
                    "File upload step failed",
                    fileUploadResult.ErrorMessage,
                    nameof(FileUploadStep)
                );
                await eventPublisher.PublishAsync(fileUploadFailureEvent, cancellationToken);
                
                return Result<PhotoDto>.Fail(Messages.Errors.Photo.FailedToAddPhoto);
            }
            executedStpes.Add(fileUploadStep);

            var metadataSaveResult = await metadataSaveStep.ExecuteAsync(sagaData, cancellationToken);
            if (!metadataSaveResult.Success)
            {
                logger.LogError("Metadata save step failed in PhotoUploadSaga {SagaId} for PhotoId {PhotoId}, Error: {Error}", sagaId, request.Photo.Id, metadataSaveResult.ErrorMessage);
                
                var metadataSaveFailureEvent = new PhotoUploadSagaFailed(
                    request.Photo.Id,
                    sagaId,
                    request.Photo.FileName,
                    request.Photo.UserId,
                    request.Photo.LighthouseId,
                    request.Photo.CameraType,
                    request.Photo.Resolution,
                    request.Photo.Lens,
                    "Metadata save step failed",
                    metadataSaveResult.ErrorMessage,
                    nameof(MetadataSaveStep)
                );
                await eventPublisher.PublishAsync(metadataSaveFailureEvent, cancellationToken);
                
                await CompensateAsync(executedStpes, sagaData, sagaId, cancellationToken);
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
                sagaData.PhotoEntity.Metadata.Lens,
                sagaData.PhotoEntity.IsPrimary
            );

            var sagaCompletedEvent = new PhotoUploadSagaCompleted(
                request.Photo.Id,
                sagaId,
                request.Photo.FileName,
                request.Photo.UserId,
                request.Photo.LighthouseId,
                request.Photo.CameraType,
                request.Photo.Resolution,
                request.Photo.Lens,
                DateTime.UtcNow
            );
            await eventPublisher.PublishAsync(sagaCompletedEvent, cancellationToken);

            var photoUploadedEvent = new PhotoUploaded(
                sagaData.PhotoEntity.Id,
                sagaData.PhotoEntity.Filename,
                sagaData.PhotoEntity.UserId,
                sagaData.PhotoEntity.LighthouseId,
                sagaData.PhotoEntity.Metadata.CameraModel,
                sagaData.PhotoEntity.Metadata.Resolution,
                sagaData.PhotoEntity.Metadata.Lens,
                sagaData.PhotoEntity.UploadDate
            );
            await eventPublisher.PublishAsync(photoUploadedEvent, cancellationToken);

            logger.LogInformation("PhotoUploadSaga {SagaId} completed successfully for PhotoId {PhotoId}", sagaId, request.Photo.Id);

            return Result<PhotoDto>.Ok(photoDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "PhotoUploadSaga {SagaId} failed for PhotoId {PhotoId}", sagaId, request.Photo.Id);
            
            var exceptionFailureEvent = new PhotoUploadSagaFailed(
                request.Photo.Id,
                sagaId,
                request.Photo.FileName,
                request.Photo.UserId,
                request.Photo.LighthouseId,
                request.Photo.CameraType,
                request.Photo.Resolution,
                request.Photo.Lens,
                "Unexpected exception occurred",
                ex.Message,
                "Unknown"
            );
            await eventPublisher.PublishAsync(exceptionFailureEvent, cancellationToken);
            
            await CompensateAsync(executedStpes, sagaData, sagaId, cancellationToken);
            return Result<PhotoDto>.Fail(Messages.Errors.Photo.FailedToAddPhoto);
        }
    }

    private async Task CompensateAsync(List<ISagaStep<PhotoUploadSagaData>> executedSteps, PhotoUploadSagaData data, Guid sagaId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting compensation for PhotoUploadSaga {SagaId} for PhotoId {PhotoId}", sagaId, data.PhotoId);

        var compensationStartedEvent = new PhotoUploadSagaCompensationStarted(
            data.PhotoId,
            sagaId,
            "Saga step failure requiring compensation"
        );
        await eventPublisher.PublishAsync(compensationStartedEvent, cancellationToken);

        for (int i = executedSteps.Count - 1; i >= 0; i--)
        {
            var step = executedSteps[i];
            try
            {
                await step.CompensateAsync(data, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Compensation failed for step {Step} in PhotoUploadSaga {SagaId} for PhotoId {PhotoId}", step.GetType().Name, sagaId, data.PhotoId);
            }
        }

        var compensationCompletedEvent = new PhotoUploadSagaCompensationCompleted(
            data.PhotoId,
            sagaId
        );
        await eventPublisher.PublishAsync(compensationCompletedEvent, cancellationToken);

        logger.LogInformation("Compensation completed for PhotoUploadSaga {SagaId} for PhotoId {PhotoId}", sagaId, data.PhotoId);
    }
}

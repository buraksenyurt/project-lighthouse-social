using LighthouseSocial.Domain.Events.Photo;

namespace LighthouseSocial.EventWorker.EventHandlers;

public class PhotoUploadedEventHandler(ILogger<PhotoUploadedEventHandler> logger)
    : IPhotoUploadedEventHandler
{
    public async Task HandleAsync(PhotoUploaded photoUploadedEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(photoUploadedEvent);

        logger.LogInformation("Handling PhotoUploaded event: PhotoId={PhotoId}, UserId={UserId}, LighthouseId={LighthouseId}, FileName={FileName}, CameraType={CameraType}, Resolution={Resolution}, Lens={Lens}, UploadedAt={UploadedAt}",
            photoUploadedEvent.AggregateId,
            photoUploadedEvent.UserId,
            photoUploadedEvent.LighthouseId,
            photoUploadedEvent.FileName,
            photoUploadedEvent.CameraType,
            photoUploadedEvent.Resolution,
            photoUploadedEvent.Lens,
            photoUploadedEvent.UploadedAt);

        //todo@buraksenyurt: implement the actual handling logic here

        await Task.CompletedTask;
    }
}

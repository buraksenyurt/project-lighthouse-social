using LighthouseSocial.Domain.Events.Photo;

namespace LighthouseSocial.EventWorker.EventHandlers;

public interface IPhotoUploadedEventHander
{
    Task HandleAsync(PhotoUploaded photoUploadedEvent, CancellationToken cancellationToken = default);
}

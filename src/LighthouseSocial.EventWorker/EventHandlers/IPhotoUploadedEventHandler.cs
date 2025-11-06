using LighthouseSocial.Domain.Events.Photo;

namespace LighthouseSocial.EventWorker.EventHandlers;

public interface IPhotoUploadedEventHandler
{
    Task HandleAsync(PhotoUploaded photoUploadedEvent, CancellationToken cancellationToken = default);
}

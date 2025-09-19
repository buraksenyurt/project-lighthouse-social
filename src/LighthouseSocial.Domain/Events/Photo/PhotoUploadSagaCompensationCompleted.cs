using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events.Photo;

public class PhotoUploadSagaCompensationCompleted(Guid photoId, Guid sagaId)
    : EventBase(photoId)
{
    public Guid SagaId { get; } = sagaId;
    public DateTime CompletedAt { get; } = DateTime.UtcNow;
}
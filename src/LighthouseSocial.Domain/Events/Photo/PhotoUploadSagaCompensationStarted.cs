using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events.Photo;

public class PhotoUploadSagaCompensationStarted(Guid photoId, Guid sagaId, string reason)
    : EventBase(photoId)
{
    public Guid SagaId { get; } = sagaId;
    public string Reason { get; } = reason ?? throw new ArgumentNullException(nameof(reason));
    public DateTime StartedAt { get; } = DateTime.UtcNow;
}
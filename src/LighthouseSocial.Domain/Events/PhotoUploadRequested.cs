using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events;

public class PhotoUploadRequested(Guid photoId, string fileName, Guid userId, Guid lighthouseId, string cameraType, string resolution, string lens)
        : EventBase(photoId)
{
    public string FileName { get; } = fileName ?? throw new ArgumentNullException(nameof(fileName));
    public Guid UserId { get; } = userId;
    public Guid LighthouseId { get; } = lighthouseId;
    public string CameraType { get; } = cameraType ?? throw new ArgumentNullException(nameof(cameraType));
    public string Resolution { get; } = resolution ?? throw new ArgumentNullException(nameof(resolution));
    public string Lens { get; } = lens ?? throw new ArgumentNullException(nameof(lens));
    public DateTime RequestedAt { get; } = DateTime.UtcNow;
}
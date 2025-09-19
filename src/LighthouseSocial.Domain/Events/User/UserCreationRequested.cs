using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events.User;

public class UserCreationRequested(Guid userId, Guid externalId, string fullname, string email, Guid? requestedBy = null)
    : EventBase(userId)
{
    public Guid ExternalId { get; } = externalId;
    public string Fullname { get; } = fullname ?? throw new ArgumentNullException(nameof(fullname));
    public string Email { get; } = email ?? throw new ArgumentNullException(nameof(email));
    public Guid? RequestedBy { get; } = requestedBy;
    public DateTime RequestedAt { get; } = DateTime.UtcNow;
}
using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events.User;

public class UserCreated(Guid userId, Guid externalId, string fullname, string email, DateTime createdAt)
    : EventBase(userId)
{
    public Guid ExternalId { get; } = externalId;
    public string Fullname { get; } = fullname ?? throw new ArgumentNullException(nameof(fullname));
    public string Email { get; } = email ?? throw new ArgumentNullException(nameof(email));
    public DateTime CreatedAt { get; } = createdAt;
}
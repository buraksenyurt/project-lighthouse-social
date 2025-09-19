using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events.User;

public class UserCreationFailed(Guid userId, Guid externalId, string fullname, string email, string errorMessage, string? errorDetails = null, Guid? requestedBy = null)
    : EventBase(userId)
{
    public Guid ExternalId { get; } = externalId;
    public string Fullname { get; } = fullname ?? throw new ArgumentNullException(nameof(fullname));
    public string Email { get; } = email ?? throw new ArgumentNullException(nameof(email));
    public string ErrorMessage { get; } = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    public string? ErrorDetails { get; } = errorDetails;
    public Guid? RequestedBy { get; } = requestedBy;
}
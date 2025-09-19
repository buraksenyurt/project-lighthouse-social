using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events;

public class CommentCreationFailed(Guid commentId, Guid userId, Guid photoId, string text, int rating, string errorMessage, string? errorDetails = null)
    : EventBase(commentId)
{
    public Guid UserId { get; } = userId;
    public Guid PhotoId { get; } = photoId;
    public string Text { get; } = text ?? throw new ArgumentNullException(nameof(text));
    public int Rating { get; } = rating;
    public string ErrorMessage { get; } = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    public string? ErrorDetails { get; } = errorDetails;
}
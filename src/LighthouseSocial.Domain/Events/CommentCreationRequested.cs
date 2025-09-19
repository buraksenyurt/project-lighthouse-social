using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events;

public class CommentCreationRequested(Guid commentId, Guid userId, Guid photoId, string text, int rating)
    : EventBase(commentId)
{
    public Guid UserId { get; } = userId;
    public Guid PhotoId { get; } = photoId;
    public string Text { get; } = text ?? throw new ArgumentNullException(nameof(text));
    public int Rating { get; } = rating;
    public DateTime RequestedAt { get; } = DateTime.UtcNow;
}
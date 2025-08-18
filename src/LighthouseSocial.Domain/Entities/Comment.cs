using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Domain.Entities;

public class Comment
    : EntityBase
{
    public Guid UserId { get; private set; }
    public Guid PhotoId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public Rating Rating { get; private set; }
    public DateTime CreatedAt { get; private set; }
    protected Comment() { }
    public Comment(Guid id, Guid userId, Guid photoId, string text, Rating rating)
    {
        Id = Guid.Empty != id ? id : Guid.NewGuid();
        UserId = userId;
        PhotoId = photoId;
        Text = text;
        Rating = rating;
        CreatedAt = DateTime.UtcNow;
    }
}

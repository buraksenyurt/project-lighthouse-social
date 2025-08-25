using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Entities;

public class User
    : EntityBase
{
    public string Fullname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Guid ExternalId { get; set; }
    protected User() { }
    public User(Guid id, Guid externalId, string fullname, string email)
    {
        Id = id != Guid.Empty ? id : Guid.NewGuid();
        ExternalId = externalId != Guid.Empty ? externalId : throw new ArgumentException("ExternalId cannot be empty", nameof(externalId));
        Fullname = fullname ?? throw new ArgumentNullException(nameof(fullname));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }
    public List<Photo> Photos { get; } = [];
    public List<Comment> Comments { get; } = [];
}

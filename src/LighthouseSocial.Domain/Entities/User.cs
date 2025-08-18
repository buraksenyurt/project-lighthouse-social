using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Entities;

public class User
    : EntityBase
{
    public string Fullname { get; set; } = null!;
    public string Email { get; set; } = null!;
    protected User() { }
    public User(Guid id, string fullname, string email)
    {
        Id = id != Guid.Empty ? id : Guid.NewGuid();
        Fullname = fullname ?? throw new ArgumentNullException(nameof(fullname));
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }
    public List<Photo> Photos { get; } = [];
    public List<Comment> Comments { get; } = [];
}

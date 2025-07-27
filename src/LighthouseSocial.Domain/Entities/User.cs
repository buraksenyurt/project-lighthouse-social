using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Entities;

public class User
    : EntityBase
{
    public string Fullname { get; set; }
    public string Email { get; set; }
    protected User() { }
    public User(Guid id, string fullname, string email)
    {
        Id = id != Guid.Empty ? id : Guid.NewGuid();
        Fullname = fullname;
        Email = email;
    }
    public List<Photo> Photos { get; } = [];
    public List<Comment> Comments { get; } = [];
}

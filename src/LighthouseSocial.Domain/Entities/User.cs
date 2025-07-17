using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Entities;

public class User
    : EntityBase
{
    public string Fullname { get; set; }
    public string Email { get; set; }
    protected User() { }
    public User(string fullname, string email)
    {
        Fullname = fullname;
        Email = email;
    }
    // Photos
    // Comments
}

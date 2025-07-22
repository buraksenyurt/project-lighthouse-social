using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Data;

public class UserRepository
    : IUserRepository
{
    public async Task<User> GetByIdAsync(Guid userId)
    {
        return new User("Can Kloud Van Dam", "can@somemail.com");
    }
}

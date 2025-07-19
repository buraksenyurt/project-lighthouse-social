using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Domain.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid userId);
}

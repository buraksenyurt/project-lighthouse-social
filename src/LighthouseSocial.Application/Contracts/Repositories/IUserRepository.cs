using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<bool> AddAsync(User user);
    Task<User> GetByIdAsync(Guid userId);
}

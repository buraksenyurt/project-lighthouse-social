using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid userId);
}

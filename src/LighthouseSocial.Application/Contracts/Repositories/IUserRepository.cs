using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<Result> AddAsync(User user);
    Task<Result<User>> GetByIdAsync(Guid userId);
    Task<Result<User>> GetBySubIdAsync(Guid subId);
    Task<Result<User>> GetByEmailAsync(string email);
}

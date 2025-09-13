using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Application.Contracts.Repositories;

public interface IUserRepository
{
    Task<Result> AddAsync(User user, CancellationToken cancellationToken = default);
    Task<Result<User>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<User>> GetBySubIdAsync(Guid subId, CancellationToken cancellationToken = default);
    Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}

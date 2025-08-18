using Dapper;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Domain.Entities;

namespace LighthouseSocial.Data.Repositories;

public class UserRepository(IDbConnectionFactory connFactory) 
    : IUserRepository
{
    private readonly IDbConnectionFactory _connFactory = connFactory;

    public async Task<User> GetByIdAsync(Guid userId)
    {
        const string sql = "SELECT id, external_id, full_name, email, joined_at FROM users WHERE id = @Id";

        using var conn = _connFactory.CreateConnection();
        var user = await conn.QuerySingleAsync<User>(sql, new { Id = userId });

        return user;
    }
}

using Dapper;
using LighthouseSocial.Application.Contracts.Repositories;

namespace LighthouseSocial.Data.Repositories;

public class UserRepository(IDbConnectionFactory connFactory)
    : IUserRepository
{
    private readonly IDbConnectionFactory _connFactory = connFactory;

    public async Task<bool> AddAsync(Domain.Entities.User user)
    {
        const string sql = "INSERT INTO users (id, external_id, full_name, email, joined_at) " +
                           "VALUES (@Id, @ExternalId, @FullName, @Email, @JoinedAt)";
        using var conn = _connFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("Id", user.Id);
        parameters.Add("ExternalId", user.ExternalId);
        parameters.Add("FullName", user.Fullname);
        parameters.Add("Email", user.Email);
        parameters.Add("JoinedAt", DateTime.UtcNow);

        var created = await conn.ExecuteAsync(sql, parameters);
        return created > 0;
    }

    public async Task<Domain.Entities.User?> GetByIdAsync(Guid userId)
    {
        const string sql = "SELECT id, external_id, full_name, email, joined_at FROM users WHERE id = @Id";

        using var conn = _connFactory.CreateConnection();
        var user = await conn.QuerySingleAsync<Domain.Entities.User>(sql, new { Id = userId });

        return user;
    }

    public async Task<Domain.Entities.User?> GetBySubIdAsync(Guid subId)
    {
        const string sql = "SELECT id, external_id, full_name, email, joined_at FROM users WHERE external_id = @ExternalId";
        using var conn = _connFactory.CreateConnection();
        var user = await conn.QuerySingleOrDefaultAsync<Domain.Entities.User>(sql, new { ExternalId = subId });
        return user;
    }

    public async Task<Domain.Entities.User?> GetByEmailAsync(string email)
    {
        const string sql = "SELECT id, external_id, full_name, email, joined_at FROM users WHERE email = @Email";
        using var conn = _connFactory.CreateConnection();
        var user = await conn.QuerySingleOrDefaultAsync<Domain.Entities.User>(sql, new { Email = email });
        return user;
    }
}

using Dapper;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;

namespace LighthouseSocial.Data.Repositories;

public class UserRepository(IDbConnectionFactory connFactory)
    : IUserRepository
{
    private readonly IDbConnectionFactory _connFactory = connFactory;

    public async Task<Result> AddAsync(Domain.Entities.User user)
    {
        try
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
            return created > 0
                ? Result.Ok()
                : Result.Fail(Messages.Errors.User.FailedToAddUser);
        }
        catch (Exception ex)
        {
            return Result.Fail($"An error occurred while adding the user: {ex.Message}");
        }
    }

    public async Task<Result<Domain.Entities.User>> GetByIdAsync(Guid userId)
    {
        try
        {
            const string sql = "SELECT id, external_id, full_name, email, joined_at FROM users WHERE id = @Id";

            using var conn = _connFactory.CreateConnection();
            var user = await conn.QuerySingleAsync<Domain.Entities.User>(sql, new { Id = userId });

            if (user == null)
            {
                return Result<Domain.Entities.User>.Fail(Messages.Errors.User.UserNotFound);
            }

            return Result<Domain.Entities.User>.Ok(user);
        }
        catch (Exception ex)
        {
            return Result<Domain.Entities.User>.Fail($"An error occurred while retrieving the user: {ex.Message}");
        }
    }

    public async Task<Result<Domain.Entities.User>> GetBySubIdAsync(Guid subId)
    {
        try
        {
            const string sql = "SELECT id, external_id, full_name, email, joined_at FROM users WHERE external_id = @ExternalId";
            using var conn = _connFactory.CreateConnection();
            var user = await conn.QuerySingleOrDefaultAsync<Domain.Entities.User>(sql, new { ExternalId = subId });

            if (user == null)
            {
                return Result<Domain.Entities.User>.Fail(Messages.Errors.User.UserNotFound);
            }

            return Result<Domain.Entities.User>.Ok(user);
        }
        catch (Exception ex)
        {
            return Result<Domain.Entities.User>.Fail($"An error occurred while retrieving the user: {ex.Message}");
        }
    }

    public async Task<Result<Domain.Entities.User>> GetByEmailAsync(string email)
    {
        try
        {
            const string sql = "SELECT id, external_id, full_name, email, joined_at FROM users WHERE email = @Email";
            using var conn = _connFactory.CreateConnection();
            var user = await conn.QuerySingleOrDefaultAsync<Domain.Entities.User>(sql, new { Email = email });

            if (user == null)
            {
                return Result<Domain.Entities.User>.Fail(Messages.Errors.User.UserNotFound);
            }

            return Result<Domain.Entities.User>.Ok(user);
        }
        catch (Exception ex)
        {
            return Result<Domain.Entities.User>.Fail($"An error occurred while retrieving the user: {ex.Message}");
        }
    }
}

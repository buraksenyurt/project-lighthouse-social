using Dapper;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Data.Repositories;

public partial class LighthouseRepository(IDbConnectionFactory connFactory, ILogger<LighthouseRepository> logger)
    : ILighthouseRepository
{
    private readonly IDbConnectionFactory _connFactory = connFactory;

    public async Task<Result> AddAsync(Lighthouse lighthouse, CancellationToken cancellationToken = default)
    {
        try
        {
            string sql = @"
            INSERT INTO lighthouses (id, name, country_id, latitude, longitude) 
            VALUES (@Id, @Name, @CountryId, @Latitude, @Longitude);";

            using var conn = _connFactory.CreateConnection();

            var added = await conn.ExecuteAsync(sql, new
            {
                lighthouse.Id,
                lighthouse.Name,
                lighthouse.CountryId,
                lighthouse.Location.Latitude,
                lighthouse.Location.Longitude
            });

            return added > 0
                ? Result.Ok()
                : Result.Fail("Failed to add lighthouse.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding lighthouse with Id {LighthouseId}", lighthouse.Id);
            return Result.Fail($"Exception occurred while adding lighthouse: {ex.Message}");
        }
    }

    public async Task<Result> UpdateAsync(Lighthouse lighthouse, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
                UPDATE lighthouses
                SET name = @Name,
                    country_id = @CountryId,
                    latitude = @Latitude,
                    longitude = @Longitude
                WHERE id = @Id;
            ";

            using var conn = _connFactory.CreateConnection();

            var updated = await conn.ExecuteAsync(sql, new
            {
                lighthouse.Id,
                lighthouse.Name,
                lighthouse.CountryId,
                lighthouse.Location.Latitude,
                lighthouse.Location.Longitude
            });

            return updated > 0
                ? Result.Ok()
                : Result.Fail("Failed to update lighthouse.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating lighthouse with Id {LighthouseId}", lighthouse.Id);
            return Result.Fail($"Exception occurred while updating lighthouse: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = "DELETE FROM lighthouses WHERE id = @Id;";
            using var conn = _connFactory.CreateConnection();
            var deleted = await conn.ExecuteAsync(sql, new { Id = id });

            return deleted > 0
                ? Result.Ok()
                : Result.Fail("Failed to delete lighthouse.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting lighthouse with Id {LighthouseId}", id);
            return Result.Fail($"Exception occurred while deleting lighthouse: {ex.Message}");
        }
    }

    public async Task<Result<Lighthouse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            string sql = @"
            SELECT l.id, l.name, l.country_id, c.name AS country_name, l.latitude, l.longitude
            FROM lighthouses l
            INNER JOIN countries c ON l.country_id = c.id
            WHERE l.id = @Id;
            ";

            using var conn = _connFactory.CreateConnection();

            var row = await conn.QuerySingleOrDefaultAsync(sql, new { Id = id });

            if (row == null)
                return Result<Lighthouse>.Fail("Lighthouse not found.");

            var country = Country.Create((int)row.country_id, (string)row.country_name);
            var coordinates = new Coordinates((double)row.latitude, (double)row.longitude);
            var lighthouse = new Lighthouse((Guid)row.id, (string)row.name, country, coordinates);

            return Result<Lighthouse>.Ok(lighthouse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving lighthouse with Id {LighthouseId}", id);
            return Result<Lighthouse>.Fail($"Exception occurred while getting lighthouse: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Lighthouse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
            SELECT l.id, l.name, l.country_id, c.name AS country_name, l.latitude, l.longitude
            FROM lighthouses l
            INNER JOIN countries c ON l.country_id = c.id;
            ";

            using var conn = _connFactory.CreateConnection();

            var rows = await conn.QueryAsync(sql);

            var list = new List<Lighthouse>();

            foreach (var row in rows)
            {
                var country = Country.Create((int)row.country_id, (string)row.country_name);
                var coordinates = new Coordinates((double)row.latitude, (double)row.longitude);
                var lighthouse = new Lighthouse((Guid)row.id, (string)row.name, country, coordinates);
                list.Add(lighthouse);
            }

            return Result<IEnumerable<Lighthouse>>.Ok(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all lighthouses");
            return Result<IEnumerable<Lighthouse>>.Fail($"Exception occurred while getting all lighthouses: {ex.Message}");
        }
    }

    public async Task<Result<(IEnumerable<Lighthouse> Lighthouses, int TotalCount)>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        try
        {
            const string countSql = "SELECT COUNT(*) FROM lighthouses;";

            const string dataSql = @"
                SELECT l.id, l.name, l.country_id, c.name AS country_name, l.latitude, l.longitude
                FROM lighthouses l
                INNER JOIN countries c ON l.country_id = c.id
                ORDER BY l.name
                OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY;
                ";

            using var conn = _connFactory.CreateConnection();
            var totalCount = await conn.QuerySingleAsync<int>(countSql);

            var rows = await conn.QueryAsync(dataSql, new { Skip = skip, Take = take });

            var list = new List<Lighthouse>();

            foreach (var row in rows)
            {
                var country = Country.Create((int)row.country_id, (string)row.country_name);
                var coordinates = new Coordinates((double)row.latitude, (double)row.longitude);
                var lighthouse = new Lighthouse((Guid)row.id, (string)row.name, country, coordinates);
                list.Add(lighthouse);
            }

            return Result<(IEnumerable<Lighthouse> Lighthouses, int TotalCount)>.Ok((list, totalCount));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving paged lighthouses with Skip {Skip} and Take {Take}", skip, take);
            return Result<(IEnumerable<Lighthouse> Lighthouses, int TotalCount)>.Fail($"Exception occurred while getting paged lighthouses: {ex.Message}");
        }
    }
}

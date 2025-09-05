using Dapper;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Data;

namespace LighthouseSocial.Data.Repositories;

public class CountryRepository(IDbConnectionFactory connFactory, ILogger<CountryRepository> logger)
    : ICountryDataReader
{
    private readonly IDbConnectionFactory _connFactory = connFactory;

    public async Task<Result<IReadOnlyList<Country>>> GetAllAsync()
    {
        try
        {
            const string sql = "SELECT id, name FROM countries ORDER BY name;";

            using var conn = _connFactory.CreateConnection();

            var rows = await conn.QueryAsync<(int id, string name)>(sql);

            var list = rows
                .Select(row => Country.Create(row.id, row.name))
                .ToList();

            return Result<IReadOnlyList<Country>>.Ok(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all countries");
            return Result<IReadOnlyList<Country>>.Fail($"Failed to get all countries: {ex.Message}");
        }
    }

    public async Task<Result<Country>> GetByIdAsync(int id)
    {
        try
        {
            const string sql = "SELECT id, name FROM countries WHERE id = @Id";

            using var conn = _connFactory.CreateConnection();

            var result = await conn.QuerySingleOrDefaultAsync<(int id, string name)>(sql, new { Id = id });

            if (result == default)
            {
                return Result<Country>.Fail($"{Messages.Errors.Country.CountryNotFound} Id: {id}");
            }

            var country = Country.Create(result.id, result.name);
            return Result<Country>.Ok(country);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving country with Id {CountryId}", id);
            return Result<Country>.Fail(ex.Message);
        }
    }
}

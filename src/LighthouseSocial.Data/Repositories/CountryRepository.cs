using Dapper;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Domain.Entities;
using System.Data;

namespace LighthouseSocial.Data.Repositories;

public class CountryRepository(IDbConnectionFactory connFactory)
    : ICountryDataReader
{
    private readonly IDbConnectionFactory _connFactory = connFactory;

    public async Task<IReadOnlyList<Country>> GetAllAsync()
    {
        const string sql = "SELECT id, name FROM countries ORDER BY name;";

        using var conn = _connFactory.CreateConnection();

        var rows = await conn.QueryAsync<(int id, string name)>(sql);

        var list = rows
            .Select(row => Country.Create(row.id, row.name))
            .ToList();

        return list;
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
            return Result<Country>.Fail(ex.Message);
        }
    }
}

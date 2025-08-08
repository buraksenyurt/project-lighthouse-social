using Dapper;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Data.Repositories;

public class LighthouseRepository(IDbConnectionFactory connFactory)
        : ILighthouseRepository
{
    private readonly IDbConnectionFactory _connFactory = connFactory;

    public async Task AddAsync(Lighthouse lighthouse)
    {
        string sql = @"
            INSERT INTO lighthouses (id, name, country_id, latitude, longitude) 
            VALUES (@Id, @Name, @CountryId, @Latitude, @Longitude);";

        using var conn = _connFactory.CreateConnection();

        await conn.ExecuteAsync(sql, new
        {
            lighthouse.Id,
            lighthouse.Name,
            lighthouse.CountryId,
            lighthouse.Location.Latitude,
            lighthouse.Location.Longitude
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM lighthouses WHERE id = @Id;";
        using var conn = _connFactory.CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<IEnumerable<Lighthouse>> GetAllAsync()
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

        return list;
    }

    public async Task<Lighthouse?> GetByIdAsync(Guid id)
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
            return null;

        var country = Country.Create((int)row.country_id, (string)row.country_name);
        var coordinates = new Coordinates((double)row.latitude, (double)row.longitude);
        return new Lighthouse((Guid)row.id, (string)row.name, country, coordinates);
    }

    public async Task UpdateAsync(Lighthouse lighthouse)
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

        await conn.ExecuteAsync(sql, new
        {
            lighthouse.Id,
            lighthouse.Name,
            lighthouse.CountryId,
            lighthouse.Location.Latitude,
            lighthouse.Location.Longitude
        });
    }
}

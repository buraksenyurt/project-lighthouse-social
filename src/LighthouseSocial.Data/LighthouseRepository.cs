using Dapper;
using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.Countries;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Data;

public class LighthouseRepository
    : ILighthouseRepository
{
    private readonly IDbConnectionFactory _connFactory;

    public LighthouseRepository(IDbConnectionFactory connFactory)
    {
        _connFactory = connFactory;
    }
    public async Task AddAsync(Lighthouse lighthouse)
    {
        string sql = @"
            INSERT INTO lighthouses (id, name, country_id, latitue, longitude) 
            VALUES (@Id, @Name, @CountryId, @Latitude, @Longitude);";

        using var conn = _connFactory.CreateConnection();

        await conn.ExecuteAsync(sql, new
        {
            Id = lighthouse.Id,
            Name = lighthouse.Name,
            CountryId = lighthouse.CountryId,
            Latitude = lighthouse.Location.Latitude,
            Longitude = lighthouse.Location.Longitude
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Lighthouse>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Lighthouse?> GetByIdAsync(Guid id)
    {
        string sql = @"
            SELECT 
                l.id, l.name, l.country_id, l.latitude, l.longitude,
                c.id AS Id, c.name AS Name
            FROM lighthouses l
            INNER JOIN countries c ON l.country_id = c.id
            WHERE l.id = @Id;
        ";

        using var conn = _connFactory.CreateConnection();

        var result = await conn.QueryAsync<Lighthouse, Country, Lighthouse>(sql,
            map: (l, c) =>
            {
                var lighthouse = new Lighthouse(l.Name, c, new Domain.ValueObjects.Coordinates(l.Location.Latitude, l.Location.Longitude));
                typeof(EntityBase).GetProperty(nameof(EntityBase.Id))?.SetValue(lighthouse, l.Id);
                return lighthouse;
            },
            param: new { Id = id },
            splitOn: "Id"
        );

        return result.FirstOrDefault();
    }

    public async Task UpdateAsync(Lighthouse lighthouse)
    {
        throw new NotImplementedException();
    }
}

using Dapper;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Data.Repositories;

public partial class LighthouseRepository
{
    public async Task<Result<IEnumerable<LighthouseWithStats>>> GetTopAsync(int count)
    {
        try
        {
            const string sql = @"
                SELECT l.id, l.name, 
	            COUNT(DISTINCT c.id) AS PhotoCount,
		            AVG(CAST(c.rating AS FLOAT)) AS AverageScore   
            FROM lighthouses l
            LEFT JOIN photos p ON l.id = p.lighthouse_id
            LEFT JOIN comments c ON p.id = c.photo_id
            GROUP BY l.id, l.name
            HAVING COUNT(DISTINCT p.id) > 0 AND COUNT(c.rating) > 0
            ORDER BY AverageScore DESC, PhotoCount DESC
            LIMIT @TopCount;
            ";

            using var conn = _connFactory.CreateConnection();
            var rows = await conn.QueryAsync<LighthouseWithStats>(sql, new { TopCount = count });

            var list = rows.Select(row => new LighthouseWithStats
            {
                Id = row.Id,
                Name = row.Name,
                AverageScore = row.AverageScore,
                PhotoCount = row.PhotoCount
            }).ToList();

            return Result<IEnumerable<LighthouseWithStats>>.Ok(list);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<LighthouseWithStats>>.Fail($"Exception occurred while getting top lighthouses: {ex.Message}");
        }
    }
}

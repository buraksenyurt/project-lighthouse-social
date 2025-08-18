using Dapper;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Application.Dtos;

namespace LighthouseSocial.Data.Repositories;

public class LighthouseODataRepository(IDbConnectionFactory connectionFactory)
	: ILighthouseODataRepository
{
    public IQueryable<QueryableLighthouseDto> GetLighthouses()
    {
        var lighthouses = GetLighthousesAsync().GetAwaiter().GetResult();
		return lighthouses.AsQueryable();
    }

    private async Task<List<QueryableLighthouseDto>> GetLighthousesAsync()
    {
        const string sql = @"
				SELECT 
						l.id,
						l.name,
						l.country_id,
						c.name as country_name,
						l.latitude,
						l.longitude,
						COALESCE(photo_stats.photo_count, 0) as photo_count,
						COALESCE(comment_stats.comment_count, 0) as comment_count,
						COALESCE(comment_stats.average_rating, 0) as average_rating,
						photo_stats.last_photo_upload_date
					FROM lighthouses l
					INNER JOIN countries c ON l.country_id = c.id
					LEFT JOIN (
						SELECT 
							lighthouse_id,
							COUNT(*) as photo_count,
							MAX(upload_date) as last_photo_upload_date
						FROM photos 
						GROUP BY lighthouse_id
					) photo_stats ON l.id = photo_stats.lighthouse_id
					LEFT JOIN (
						SELECT 
							p.lighthouse_id,
							COUNT(cm.id) as comment_count,
							AVG(CAST(cm.rating as DECIMAL(3,2))) as average_rating
						FROM photos p
						LEFT JOIN comments cm ON p.id = cm.photo_id
						GROUP BY p.lighthouse_id
					) comment_stats ON l.id = comment_stats.lighthouse_id
				ORDER BY l.name
				";

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync(sql);

        return [.. rows.Select(row => new QueryableLighthouseDto(
            Id: (Guid)row.id,
            Name: (string)row.name,
            CountryId: (int)row.country_id,
            CountryName: (string)row.country_name,
            Latitude: (double)row.latitude,
            Longitude: (double)row.longitude,
            PhotoCount: (int)row.photo_count,
            CommentCount: (int)row.comment_count,
            AverageRating: row.average_rating != null ? (double)row.average_rating : 0,
            LastPhotoUploadDate: row.last_photo_upload_date as DateTime?
            ))];
    }
}

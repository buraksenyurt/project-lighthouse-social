using Dapper;
using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.Interfaces;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Data.Repositories;

public class PhotoRepository(IDbConnectionFactory connFactory)
    : IPhotoRepository
{
    private readonly IDbConnectionFactory _connFactory = connFactory;

    public async Task AddAsync(Photo photo)
    {
        const string sql = @"
            INSERT INTO photos (id, user_id, lighthouse_id, filename, upload_date, lens, resolution, camera_model, taken_at)
            VALUES (@Id, @UserId, @LighthouseId, @FileName, @UploadDate, @Lens, @Resolution, @CameraModel, @TakenAt)";

        using var conn = _connFactory.CreateConnection();
        var metadata = photo.Metadata;

        await conn.ExecuteAsync(sql, new
        {
            photo.Id,
            photo.UserId,
            photo.LighthouseId,
            photo.Filename,
            photo.UploadDate,
            metadata.Lens,
            metadata.Resolution,
            metadata.CameraModel,
            metadata.TakenAt
        });
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = "DELETE FROM photos WHERE id = @Id";

        using var conn = _connFactory.CreateConnection();

        await conn.ExecuteAsync(sql, new { Id = id });
    }

    public async Task<Photo?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT id, user_id, lighthouse_id, filename, upload_date, lens, resolution, camera_model, taken_at 
            FROM photos 
            WHERE id = @Id";

        using var conn = _connFactory.CreateConnection();

        var photoDto = await conn.QuerySingleOrDefaultAsync(sql, new { Id = id });
        return photoDto is null ? null : MapPhoto(photoDto);
    }

    public async Task<IEnumerable<Photo>> GetByLighthouseIdAsync(Guid lighthouseId)
    {
        const string sql = @"
            SELECT id, user_id, lighthouse_id, filename, upload_date, lens, resolution, camera_model, taken_at 
            FROM photos 
            WHERE lighthouse_id = @LighthouseId 
            ORDER BY upload_date DESC";

        using var conn = _connFactory.CreateConnection();

        var rows = await conn.QueryAsync(sql, new { LighthouseId = lighthouseId });
        return rows.Select(MapPhoto);
    }

    public async Task<IEnumerable<Photo>> GetByUserIdAsync(Guid userId)
    {
        const string sql = @"
            SELECT id, user_id, lighthouse_id, filename, upload_date, lens, resolution, camera_model, taken_at
            FROM photos 
            WHERE user_id = @UserId 
            ORDER BY upload_date DESC";

        using var conn = _connFactory.CreateConnection();

        var rows = await conn.QueryAsync(sql, new { UserId = userId });
        return rows.Select(MapPhoto);
    }

    private static Photo MapPhoto(dynamic row)
    {
        var metadata = new PhotoMetadata((string)row.lens, (string)row.resolution, (string)row.camera_model, (DateTime)row.taken_at);
        var photo = new Photo((Guid)row.id, (Guid)row.user_id, (Guid)row.lighthouse_id, (string)row.filename, metadata);
        //todo@buraksenyurt Refection ile ID atamayı terk edelim
        typeof(EntityBase).GetProperty(nameof(EntityBase.Id))?.SetValue(photo, (Guid)row.Id);
        return photo;
    }
}
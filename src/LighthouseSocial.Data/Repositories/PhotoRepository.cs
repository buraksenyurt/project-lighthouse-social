using Dapper;
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts.Repositories;
using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace LighthouseSocial.Data.Repositories;

public class PhotoRepository(IDbConnectionFactory connFactory, ILogger<PhotoRepository> logger)
    : IPhotoRepository
{
    private readonly IDbConnectionFactory _connFactory = connFactory;

    public async Task<Result> AddAsync(Photo photo, CancellationToken cancellationToken = default)
    {
        try
        {
            // throw new ArgumentException(); SAGA Testi için eklendi.

            const string sql = @"
            INSERT INTO photos (id, user_id, lighthouse_id, filename, upload_date, lens, resolution, camera_model, taken_at, is_primary)
            VALUES (@Id, @UserId, @LighthouseId, @FileName, @UploadDate, @Lens, @Resolution, @CameraModel, @TakenAt, @IsPrimary);";

            using var conn = _connFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("Id", photo.Id);
            parameters.Add("UserId", photo.UserId);
            parameters.Add("LighthouseId", photo.LighthouseId);
            parameters.Add("FileName", photo.Filename);
            parameters.Add("UploadDate", photo.UploadDate);
            parameters.Add("Lens", photo.Metadata?.Lens);
            parameters.Add("Resolution", photo.Metadata?.Resolution);
            parameters.Add("CameraModel", photo.Metadata?.CameraModel);
            parameters.Add("TakenAt", photo.Metadata?.TakenAt);
            parameters.Add("IsPrimary", photo.IsPrimary);

            var added = await conn.ExecuteAsync(sql, parameters);

            return added > 0
                ? Result.Ok()
                : Result.Fail(Messages.Errors.Photo.FailedToAddPhoto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding photo with Id {PhotoId}", photo.Id);
            return Result.Fail($"Exception occured while adding photo: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = "DELETE FROM photos WHERE id = @Id;";

            using var conn = _connFactory.CreateConnection();

            var deleted = await conn.ExecuteAsync(sql, new { Id = id });

            return deleted > 0
                ? Result.Ok()
                : Result.Fail(Messages.Errors.Photo.FailedToDeletePhoto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting photo with Id {PhotoId}", id);
            return Result.Fail($"Exception occured while deleting photo: {ex.Message}");
        }
    }

    public async Task<Result<Photo>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
            SELECT id, user_id, lighthouse_id, filename, upload_date, lens, resolution, camera_model, taken_at, is_primary 
            FROM photos 
            WHERE id = @Id;";

            using var conn = _connFactory.CreateConnection();

            var photoDto = await conn.QuerySingleOrDefaultAsync(sql, new { Id = id });
            if (photoDto == null)
                return Result<Photo>.Fail(Messages.Errors.Photo.PhotoNotFound);

            var photo = MapPhoto(photoDto);
            return Result<Photo>.Ok(photo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving photo with Id {PhotoId}", id);
            return Result<Photo>.Fail($"Exception occured while getting photo: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Photo>>> GetByLighthouseIdAsync(Guid lighthouseId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
            SELECT id, user_id, lighthouse_id, filename, upload_date, lens, resolution, camera_model, taken_at, is_primary 
            FROM photos 
            WHERE lighthouse_id = @LighthouseId 
            ORDER BY upload_date DESC;";

            using var conn = _connFactory.CreateConnection();

            var rows = await conn.QueryAsync(sql, new { LighthouseId = lighthouseId });
            var photos = rows.Select(MapPhoto).ToList();

            return Result<IEnumerable<Photo>>.Ok(photos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving photos for LighthouseId {LighthouseId}", lighthouseId);
            return Result<IEnumerable<Photo>>.Fail($"Exception occured while getting photos by lighthouse: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Photo>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            const string sql = @"
            SELECT id, user_id, lighthouse_id, filename, upload_date, lens, resolution, camera_model, taken_at, is_primary
            FROM photos 
            WHERE user_id = @UserId 
            ORDER BY upload_date DESC;";

            using var conn = _connFactory.CreateConnection();

            var rows = await conn.QueryAsync(sql, new { UserId = userId });
            var photos = rows.Select(MapPhoto).ToList();

            return Result<IEnumerable<Photo>>.Ok(photos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving photos for UserId {UserId}", userId);
            return Result<IEnumerable<Photo>>.Fail($"Exception occured while getting photos by user: {ex.Message}");
        }
    }

    private static Photo MapPhoto(dynamic row)
    {
        var metadata = new PhotoMetadata((string)row.lens, (string)row.resolution, (string)row.camera_model, (DateTime)row.taken_at);
        var photo = new Photo((Guid)row.id, (Guid)row.user_id, (Guid)row.lighthouse_id, (string)row.filename, metadata, (bool)row.is_primary);
        //todo@buraksenyurt Refection ile ID atamayı terk edelim
        typeof(EntityBase).GetProperty(nameof(EntityBase.Id))?.SetValue(photo, (Guid)row.id);
        return photo;
    }
}
using LighthouseSocial.Application.Common;
using LighthouseSocial.Application.Contracts;
using LighthouseSocial.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace LighthouseSocial.Infrastructure.Storage;

public class PhotoStorageService
    : IPhotoStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucket;
    private readonly ILogger<PhotoStorageService> _logger;
    public PhotoStorageService(IOptions<MinioSettings> options, CachedConfigurationService configurationService, ILogger<PhotoStorageService> logger)
    {
        _logger = logger;
        var credentials = configurationService.GetMinioCredentialsAsync().GetAwaiter().GetResult();

        var settings = options.Value;
        _bucket = settings.BucketName;
        _minioClient = new MinioClient()
            .WithEndpoint(settings.Endpoint)
            .WithCredentials(credentials.AccessKey, credentials.SecretKey)
            .WithSSL(settings.UseSSL)
            .Build();
        _logger = logger;
    }
    public async Task<Result> DeleteAsync(string filePath)
    {
        try
        {
            await _minioClient.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(filePath));

            _logger.LogInformation("Photo {FilePath} deleted from MinIO bucket {BucketName}", filePath, _bucket);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting photo. FilePath: {FilePath}, Bucket: {BucketName}", filePath, _bucket);
            return Result.Fail($"Failed to delete photo: {ex.Message}");
        }
    }

    public async Task<Result<Stream>> GetAsync(string filePath)
    {
        try
        {
            _logger.LogDebug("Attempting to retrieve photo from MinIO. FilePath: {FilePath}, Bucket: {BucketName}", filePath, _bucket);

            bool bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
            if (!bucketExists)
            {
                _logger.LogError("Bucket {BucketName} does not exist", _bucket);
                return Result<Stream>.Fail($"Bucket '{_bucket}' does not exist");
            }

            var ms = new MemoryStream();
            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(filePath)
                    .WithCallbackStream(stream =>
                    {
                        stream.CopyTo(ms);
                        _logger.LogDebug("Copied {Bytes} bytes to memory stream for file {FilePath}", ms.Length, filePath);
                    }));

            ms.Position = 0;

            if (ms.Length == 0)
            {
                _logger.LogWarning("Retrieved photo {FilePath} from MinIO but stream is empty", filePath);
                return Result<Stream>.Fail($"Photo '{filePath}' exists but contains no data");
            }

            _logger.LogInformation("Photo {FilePath} retrieved successfully from MinIO bucket {BucketName}. Size: {Size} bytes", filePath, _bucket, ms.Length);
            return Result<Stream>.Ok(ms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting photo. FilePath: {FilePath}, Bucket: {BucketName}. Exception Type: {ExceptionType}, Message: {Message}",
                filePath, _bucket, ex.GetType().Name, ex.Message);
            return Result<Stream>.Fail($"Failed to get photo: {ex.Message}");
        }
    }

    public async Task<Result<string>> SaveAsync(Stream content, string fileName)
    {
        try
        {
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
            if (!found)
            {
                _logger.LogWarning("Bucket {BucketName} does not exist. Creating it now.", _bucket);
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));
            }

            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(fileName)
                    .WithStreamData(content)
                    .WithObjectSize(content.Length)
                    .WithContentType("application/octet-stream"));

            _logger.LogInformation("Photo {FileName} saved to MinIO bucket {BucketName}", fileName, _bucket);

            return Result<string>.Ok(fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while saving photo. FileName: {FileName}, Bucket: {BucketName}", fileName, _bucket);
            return Result<string>.Fail($"Failed to save photo: {ex.Message}");
        }

    }
}

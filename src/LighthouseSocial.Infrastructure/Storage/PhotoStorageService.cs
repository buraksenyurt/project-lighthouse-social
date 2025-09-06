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
            var ms = new MemoryStream();
            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(filePath)
                    .WithCallbackStream(stream => stream.CopyTo(ms)));
            ms.Position = 0;
            _logger.LogInformation("Photo {FilePath} retrieved from MinIO bucket {BucketName}", filePath, _bucket);
            return Result<Stream>.Ok(ms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting photo. FilePath: {FilePath}, Bucket: {BucketName}", filePath, _bucket);
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

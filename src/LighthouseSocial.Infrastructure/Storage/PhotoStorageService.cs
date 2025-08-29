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
        var (accessKey, secretKey) = configurationService.GetMinioCredentialsAsync().GetAwaiter().GetResult();

        var settings = options.Value;
        _bucket = settings.BucketName;
        _minioClient = new MinioClient()
            .WithEndpoint(settings.Endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(settings.UseSSL)
            .Build();
        _logger = logger;
    }
    public async Task DeleteAsync(string filePath)
    {
        await _minioClient.RemoveObjectAsync(
            new RemoveObjectArgs()
                .WithBucket(_bucket)
                .WithObject(filePath));
    }

    public async Task<Stream> GetAsync(string filePath)
    {
        var ms = new MemoryStream();
        await _minioClient.GetObjectAsync(
            new GetObjectArgs()
                .WithBucket(_bucket)
                .WithObject(filePath)
                .WithCallbackStream(stream => stream.CopyTo(ms)));
        ms.Position = 0;
        return ms;
    }

    public async Task<string> SaveAsync(Stream content, string fileName)
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

        return $"{fileName}";
    }
}

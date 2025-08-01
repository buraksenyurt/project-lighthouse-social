using LighthouseSocial.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace LighthouseSocial.Infrastructure.Storage
{
    public class PhotoStorageService
        : IPhotoStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucket;
        public PhotoStorageService(IOptions<MinioSettings> options)
        {
            var settings = options.Value;
            _bucket = settings.BucketName;
            _minioClient = new MinioClient()
                .WithEndpoint(settings.Endpoint)
                .WithCredentials(settings.AccessKey, settings.SecretKey)
                .WithSSL(settings.UseSSL)
                .Build();
        }
        public Task DeleteAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SaveAsync(Stream content, string fileName)
        {
            bool found = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
            if(!found)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));
            }

            await _minioClient.PutObjectAsync(
                new PutObjectArgs()
                    .WithBucket(_bucket)
                    .WithObject(fileName)
                    .WithStreamData(content)
                    .WithObjectSize(content.Length)
                    .WithContentType("application/octet-stream"));

            return $"{_bucket}/{fileName}";
        }
    }
}

using LighthouseSocial.Application.Common;

namespace LighthouseSocial.Application.Contracts;

public interface IPhotoStorageService
{
    Task<Result<Stream>> GetAsync(string filePath, CancellationToken cancellationToken = default);
    Task<Result<string>> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string filePath, CancellationToken cancellationToken = default);
}

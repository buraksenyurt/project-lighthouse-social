using LighthouseSocial.Application.Common;

namespace LighthouseSocial.Application.Contracts;

public interface IPhotoStorageService
{
    Task<Result<Stream>> GetAsync(string filePath);
    Task<Result<string>> SaveAsync(Stream content, string fileName);
    Task<Result> DeleteAsync(string filePath);
}

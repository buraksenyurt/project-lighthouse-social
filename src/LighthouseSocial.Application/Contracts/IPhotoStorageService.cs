namespace LighthouseSocial.Application.Contracts;

public interface IPhotoStorageService
{
    Task<Stream> GetAsync(string filePath);
    Task<string> SaveAsync(Stream content, string fileName);
    Task DeleteAsync(string filePath);
}

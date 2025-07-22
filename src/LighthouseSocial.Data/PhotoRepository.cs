using LighthouseSocial.Domain.Entities;
using LighthouseSocial.Domain.Interfaces;

namespace LighthouseSocial.Data;

public class PhotoRepository
    : IPhotoRepository
{
    public async Task AddAsync(Photo photo)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Photo?> GetByIdAsync(Guid id)
    {
        return
            new Photo(
                 Guid.NewGuid(),
                 Guid.NewGuid(),
                "EndOfTheWorld.jpg",
                new Domain.ValueObjects.PhotoMetadata(
                    "50mm", "1280x1280", "Canon Mark 5", DateTime.Now.AddDays(-7)));
    }

    public async Task<IEnumerable<Photo>> GetByLighthouseIdAsync(Guid lighthouseId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Photo>> GetByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}

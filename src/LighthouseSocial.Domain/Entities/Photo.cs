using LighthouseSocial.Domain.Common;
using LighthouseSocial.Domain.ValueObjects;

namespace LighthouseSocial.Domain.Entities;

public class Photo
    : EntityBase
{
    public Guid UserId { get; private set; }
    public Guid LighthouseId { get; private set; }
    public string Filename { get; private set; } = null!;
    public DateTime UploadDate { get; private set; }
    public PhotoMetadata Metadata { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public List<Comment> Comments { get; } = [];

    protected Photo() { }

    public Photo(Guid id, Guid userId, Guid lighthouseId, string filename, PhotoMetadata metadata, bool isPrimary = false)
    {
        Id = id != Guid.Empty ? id : Guid.NewGuid();
        UserId = userId;
        LighthouseId = lighthouseId;
        Filename = filename ?? throw new ArgumentNullException(nameof(filename));
        UploadDate = DateTime.UtcNow;
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        IsPrimary = isPrimary;
    }

    public void SetFileName(string fileName)
    {
        Filename = fileName;
    }

    public void SetIsPrimary(bool isPrimary)
    {
        IsPrimary = isPrimary;
    }
}

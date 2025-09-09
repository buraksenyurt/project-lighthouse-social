namespace LighthouseSocial.Application.Features.Photo.Saga;

public class PhotoUploadSagaData
{
    public Guid PhotoId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public Stream? FileStream { get; set; }

    public bool IsFileUploaded { get; set; }
    public bool IsMetadataSaved { get; set; }

    public Domain.Entities.Photo? PhotoEntity { get; set; }
    public Exception? LastException { get; set; }
}

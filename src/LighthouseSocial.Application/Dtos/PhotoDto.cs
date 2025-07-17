namespace LighthouseSocial.Application.Dtos;

public record PhotoDto(Guid Id, string FileName, DateTime UploadedAt, string CameraModel, Guid UserId, Guid LighthouseId);
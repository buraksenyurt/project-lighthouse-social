namespace LighthouseSocial.Application.Dtos;

public record PhotoDto(Guid Id, string FileName, DateTime UploadedAt, string CameraType, Guid UserId, Guid LighthouseId);
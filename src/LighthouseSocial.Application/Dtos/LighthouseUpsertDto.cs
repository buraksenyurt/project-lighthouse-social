namespace LighthouseSocial.Application.Dtos;

public record LighthouseUpsertDto(Guid Id, string Name, int CountryId, double Latitude, double Longitude);

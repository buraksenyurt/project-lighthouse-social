namespace LighthouseSocial.Application.Dtos;

public record LighthouseDto(Guid Id, string Name, int CountryId, double Latitude, double Longitude);

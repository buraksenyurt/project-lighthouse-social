namespace LighthouseSocial.Application.Dtos;

public record LighthouseDto(Guid Id, string Name, int CountryId, string CountryName, double Latitude, double Longitude);

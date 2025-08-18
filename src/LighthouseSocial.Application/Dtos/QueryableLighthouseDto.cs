namespace LighthouseSocial.Application.Dtos;

public record QueryableLighthouseDto(
    Guid Id,
    string Name,
    int CountryId,
    string CountryName,
    double Latitude,
    double Longitude,
    int PhotoCount,
    int CommentCount,
    double AverageRating,
    DateTime? LastPhotoUploadDate
);


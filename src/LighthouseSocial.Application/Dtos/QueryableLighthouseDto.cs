using System.ComponentModel.DataAnnotations;

namespace LighthouseSocial.Application.Dtos;

public record QueryableLighthouseDto(
    [property: Key] Guid Id,
    string Name,
    int CountryId,
    string CountryName,
    double Latitude,
    double Longitude,
    int PhotoCount,
    int CommentCount,
    double AverageRating,
    DateTime? LastPhotoUploadDate);

public static class QueryableLighthouseDtoExtensions
{
    public static QueryableLighthouseDto ToQueryableDto(this LighthouseDto lighthouse,
        string countryName = "",
        int photoCount = 0,
        int commentCount = 0,
        double averageRating = 0,
        DateTime? lastPhotoUploadDate = null)
    {
        return new QueryableLighthouseDto(
            lighthouse.Id,
            lighthouse.Name,
            lighthouse.CountryId,
            countryName,
            lighthouse.Latitude,
            lighthouse.Longitude,
            photoCount,
            commentCount,
            averageRating,
            lastPhotoUploadDate);
    }
}


namespace LighthouseSocial.Backoffice.Models;

public record CreateLighthouseRequest(
    string Name,
    int CountryId,
    double Latitude,
    double Longitude
);

public record LighthouseDto(
    Guid Id,
    string Name,
    int CountryId,
    string CountryName,
    double Latitude,
    double Longitude
);

public record CountryDto(
    int Id,
    string Name
);

public record PhotoUploadRequest(
    string FileName,
    string CameraType,
    Guid UserId,
    Guid LighthouseId,
    string Resolution,
    string Lens
);

public record ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}

public record PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

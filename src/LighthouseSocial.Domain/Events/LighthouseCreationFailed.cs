using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events;

public class LighthouseCreationFailed
    : EventBase
{
    public string Name { get; }
    public int CountryId { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public string ErrorMessage { get; }
    public string? ErrorDetails { get; }
    public Guid RequestedBy { get; }

    public LighthouseCreationFailed(Guid lighthouseId, string name, int countryId, double latitude, double longitude, string errorMessage, string? errorDetails, Guid requestedBy)
        : base(lighthouseId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        CountryId = countryId;
        Latitude = latitude;
        Longitude = longitude;
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        ErrorDetails = errorDetails;
        RequestedBy = requestedBy;
    }
}

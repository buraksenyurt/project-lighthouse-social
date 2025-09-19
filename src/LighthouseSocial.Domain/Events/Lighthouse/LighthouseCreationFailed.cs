using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events.Lighthouse;

public class LighthouseCreationFailed(Guid lighthouseId, string name, int countryId, double latitude, double longitude, string errorMessage, string? errorDetails, Guid requestedBy)
    : EventBase(lighthouseId)
{
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
    public int CountryId { get; } = countryId;
    public double Latitude { get; } = latitude;
    public double Longitude { get; } = longitude;
    public string ErrorMessage { get; } = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    public string? ErrorDetails { get; } = errorDetails;
    public Guid RequestedBy { get; } = requestedBy;
}

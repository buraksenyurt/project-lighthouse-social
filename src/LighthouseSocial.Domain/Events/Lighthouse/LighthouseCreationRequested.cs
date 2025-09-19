using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events.Lighthouse;

public class LighthouseCreationRequested(Guid lighthouseId, string name, int countryId, double latitude, double longitude, Guid requestedBy)
    : EventBase(lighthouseId)
{
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
    public int CountryId { get; } = countryId;
    public double Latitude { get; } = latitude;
    public double Longitude { get; } = longitude;
    public Guid RequestedBy { get; } = requestedBy;
}

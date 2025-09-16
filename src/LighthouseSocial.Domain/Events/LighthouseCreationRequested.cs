using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events;

public class LighthouseCreationRequested
    : EventBase
{
    public string Name { get; }
    public int CountryId { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public Guid RequestedBy { get; }
    public LighthouseCreationRequested(Guid lighthouseId, string name, int countryId, double latitude, double longitude, Guid requestedBy)
        : base(lighthouseId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        CountryId = countryId;
        Latitude = latitude;
        Longitude = longitude;
        RequestedBy = requestedBy;
    }
}

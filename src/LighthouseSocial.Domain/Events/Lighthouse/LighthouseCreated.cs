using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events.Lighthouse;

public class LighthouseCreated(Guid lighthouseId, string name, int countryId, string countryName, double latitude, double longitude, DateTime createdAt)
    : EventBase(lighthouseId)
{
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));
    public int CountryId { get; } = countryId;
    public string CountryName { get; } = countryName ?? throw new ArgumentNullException(nameof(countryName));
    public double Latitude { get; } = latitude;
    public double Longitude { get; } = longitude;
    public DateTime CreatedAt { get; } = createdAt;
}

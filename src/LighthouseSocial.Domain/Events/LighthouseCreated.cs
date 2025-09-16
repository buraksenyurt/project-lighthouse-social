using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Events;

public class LighthouseCreated
    : EventBase
{
    public string Name { get; }
    public int CountryId { get; }
    public string CountryName { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public DateTime CreatedAt { get; }
    public LighthouseCreated(Guid lighthouseId, string name, int countryId, string countryName, double latitude, double longitude, DateTime createdAt)
        : base(lighthouseId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        CountryId = countryId;
        CountryName = countryName ?? throw new ArgumentNullException(nameof(countryName));
        Latitude = latitude;
        Longitude = longitude;
        CreatedAt = createdAt;
    }
}

using LighthouseSocial.Domain.Common;

namespace LighthouseSocial.Domain.Enumerations;

public sealed class CameraType
    : EnumerationBase
{
    private CameraType(int id, string name) : base(id, name) { }
    public static readonly CameraType SLR = new(1, "SLR");
    public static readonly CameraType DSLR = new(2, "DSLR");
    public static readonly CameraType Mirrorless = new(3, "Mirrorless");
    public static readonly CameraType Phone = new(4, "Phone");
    public static IEnumerable<CameraType> List() => [SLR, DSLR, Mirrorless, Phone];
    public static CameraType FromId(int id) => List().FirstOrDefault(c => c.Id == id) ?? throw new ArgumentOutOfRangeException(nameof(id));
    public static CameraType FromName(string name)
    {
        return List()
            .FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Invalid name: {name}");
    }
}

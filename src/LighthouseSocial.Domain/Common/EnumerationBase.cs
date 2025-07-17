namespace LighthouseSocial.Domain.Common;

public abstract class EnumerationBase(int id, string name)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public override string ToString() => Name;
}

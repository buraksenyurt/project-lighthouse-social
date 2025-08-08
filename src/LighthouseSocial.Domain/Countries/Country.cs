namespace LighthouseSocial.Domain.Countries;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
    public override string ToString() => Name;
    public Country()
    {
        
    }

    internal Country(int id, string name)
    {
        Id = id;
        Name = name;
    }
    public static Country Create(int id, string name) => new(id, name);
}

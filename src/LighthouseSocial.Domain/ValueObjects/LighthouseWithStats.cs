namespace LighthouseSocial.Domain.ValueObjects;

public class LighthouseWithStats
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double AverageScore { get; set; }
    public int PhotoCount { get; set; }
}

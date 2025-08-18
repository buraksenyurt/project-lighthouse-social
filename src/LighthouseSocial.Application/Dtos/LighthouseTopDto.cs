namespace LighthouseSocial.Application.Dtos;

public class LighthouseTopDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public double AverageScore { get; set; }
    public int PhotoCount { get; set; }
}

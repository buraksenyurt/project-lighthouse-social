namespace LighthouseSocial.Application.Dtos;

public record TopDto
{
    public int Count { get; }

    public TopDto(int count)
    {
        if (count < 3 || count > 25)
            throw new ArgumentOutOfRangeException(nameof(count), "Count must be between 3 and 25.");

        Count = count;
    }
}


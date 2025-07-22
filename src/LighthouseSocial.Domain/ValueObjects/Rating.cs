namespace LighthouseSocial.Domain.ValueObjects;

public readonly record struct Rating(int Value)
{
    public static Rating FromValue(int value)
    {
        if (value < 1 || value > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Rating must be between 1 and 10.");
        }
        return new Rating(value);
    }
}
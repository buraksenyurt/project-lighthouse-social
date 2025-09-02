namespace LighthouseSocial.Infrastructure.Configuration;

public class GraylogSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 12201;
    public string Facility { get; set; } = "lighthouse-social";
    public string Environment { get; set; } = "development";
    public bool UseSecureConnection { get; set; } = false;
}

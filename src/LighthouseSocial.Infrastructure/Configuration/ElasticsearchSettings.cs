namespace LighthouseSocial.Infrastructure.Configuration;

public class ElasticsearchSettings
{
    public string Uri { get; set; } = "http://localhost:9200";
    public string IndexFormat { get; set; } = "lighthouse-social-logs-{0:yyyy.MM.dd}";
    public string Username { get; set; } =string.Empty;
    public string Password { get; set; } = string.Empty;
}

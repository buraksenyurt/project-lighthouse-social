namespace LighthouseSocial.Infrastructure.Configuration;

public class RabbitMqSettings
{
    public const string SectionName = "RabbitMQ";
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = "lighthouse-project-events";
    public string QueueName { get; set; } = "lighthouse-project-queue";
    public bool Durable { get; set; } = true;
    public bool AutoDelete { get; set; } = false;
}

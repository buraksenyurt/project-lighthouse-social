using LighthouseSocial.EventWorker.Services;

namespace LighthouseSocial.EventWorker.Strategies;

public interface IEventStrategy
{
    public string EventType { get; set; }
    public Task HandleEventAsync(RabbitMqEventConsumerService.EventMessage eventMessage, CancellationToken cancellationToken);
}
using LighthouseSocial.EventWorker.Services;

namespace LighthouseSocial.EventWorker.Strategies;

public class EventDispatcher
{
    private readonly Dictionary<string, IEventStrategy> _strategies;

    public EventDispatcher(IEnumerable<IEventStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.EventType, s => s);
    }


    public async Task DispatchAsync(RabbitMqEventConsumerService.EventMessage eventMessage, CancellationToken cancellationToken)
    {
        if (_strategies.TryGetValue(eventMessage.EventType, out var strategy))
        {
            await strategy.HandleEventAsync(eventMessage, cancellationToken);
        }
        //log: no strategy about eventType
    }
}
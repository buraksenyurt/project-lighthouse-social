using LighthouseSocial.EventWorker.Services;

namespace LighthouseSocial.EventWorker.Strategies;

public class EventDispatcher
{
    private readonly Dictionary<string, IEventStrategy> _strategies;

    public EventDispatcher(IEnumerable<IEventStrategy> strategies)
    {
        // Validate for duplicate EventType values
        var duplicateEventTypes = strategies
            .GroupBy(s => s.EventType)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateEventTypes.Any())
        {
            throw new InvalidOperationException(
                $"Duplicate EventType(s) found in strategies: {string.Join(", ", duplicateEventTypes)}"
            );
        }
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
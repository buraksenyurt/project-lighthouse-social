namespace LighthouseSocial.Domain.Common;

public interface IEventPublisher
{
    Task PublishAsync(IEvent @event, CancellationToken cancellationToken = default);
    Task PublishAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);
}

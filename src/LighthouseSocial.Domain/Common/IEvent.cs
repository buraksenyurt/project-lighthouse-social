namespace LighthouseSocial.Domain.Common;

public interface IEvent
{
    Guid EventId { get; }
    DateTime OccuredAt { get; }
    string EventType { get; }
    Guid AggregateId { get; }
}

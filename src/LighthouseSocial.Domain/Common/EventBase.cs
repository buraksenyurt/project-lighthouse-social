namespace LighthouseSocial.Domain.Common;

public abstract class EventBase 
    : IEvent
{
    public Guid EventId { get; protected set; }
    public DateTime OccuredAt { get; protected set; }
    public string EventType { get; protected set; }
    public Guid AggregateId { get; protected set; }

    protected EventBase(Guid aggregateId)
    {
        EventId = Guid.NewGuid();
        OccuredAt = DateTime.UtcNow;
        EventType = GetType().Name;
        AggregateId = aggregateId;
    }
}

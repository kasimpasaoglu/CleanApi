using Application.Events.Base;

namespace Application.Events.SampleEntityCreated;

public class SampleEntityCreatedDomainEvent(Guid id, string name) : MediatrDomainEvent
{
    public Guid Id { get; } = id;
    public string? Name { get; } = name;
}
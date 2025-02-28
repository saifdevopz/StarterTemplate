namespace Common.Domain.Abstractions;

public abstract class BaseEntity
{
    public bool IsActive { get; private set; } = true;
    public IReadOnlyCollection<IDomainEvent> DomainEvents => [.. _domainEvents];
    private readonly List<IDomainEvent> _domainEvents = [];

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

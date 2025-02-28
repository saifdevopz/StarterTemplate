using Common.Domain.Abstractions;

namespace Parent.Domain.Inventory.CategoryGroup;

public sealed class CategoryGroupCreatedDomainEvent(string categoryGroupCode, string categoryGroupDesc) : DomainEvent
{
    public string CategoryGroupCode { get; init; } = categoryGroupCode;
    public string CategoryGroupDesc { get; init; } = categoryGroupDesc;
}
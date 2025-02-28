using Common.Domain.Abstractions;

namespace Parent.Domain.Inventory.CategoryGroup;

public sealed class CategoryGroupM : AggregateRoot
{
    public int CategoryGroupId { get; }
    public string CategoryGroupCode { get; private set; } = string.Empty;
    public string CategoryGroupDesc { get; private set; } = string.Empty;

    public static CategoryGroupM Create
    (
        string categoryGroupCode,
        string categoryGroupDesc
    )
    {
        CategoryGroupM categoryGroup = new()
        {
            CategoryGroupCode = categoryGroupCode,
            CategoryGroupDesc = categoryGroupDesc,
        };

        categoryGroup.AddDomainEvent(new CategoryGroupCreatedDomainEvent(categoryGroup.CategoryGroupCode, categoryGroup.CategoryGroupDesc));

        return categoryGroup;
    }

}
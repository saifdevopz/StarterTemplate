using Common.Application.Messaging;
using Common.Domain.TransferObjects.Inventory;
using Parent.Domain.Inventory.CategoryGroup;

namespace Parent.Application.Inventory.CategoryGroup.CreateCategoryGroup;

public sealed record CreateCategoryGroupCommand(WriteCategoryGroup Request)
    : ICommand<CategoryGroupM>;
namespace Common.Domain.TransferObjects.Inventory;

public sealed record ReadCategoryGroup
(
    int TenantTypeId,
    string TenantTypeCode,
    string TenantTypeName
);

public sealed record WriteCategoryGroup
(
    string CategoryGroupCode,
    string CategoryGroupDesc
);
namespace Blazor.Common.DataTransfers;
public sealed record GetAllTenants
(
    int TenantId,
    string TenantTypeCode,
    string TenantTypeName
);

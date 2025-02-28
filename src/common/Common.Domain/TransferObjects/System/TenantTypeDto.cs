namespace Common.Domain.TransferObjects.System;

public sealed record ReadTenantTypeDto
(
    int TenantTypeId,
    string TenantTypeCode,
    string TenantTypeName
);

public sealed record WriteTenantTypeDto
(
    int TenantTypeId,
    string TenantTypeCode,
    string TenantTypeDesc
);

public class WriteTenantType
{
    public int TenantTypeId { get; set; }
    public string TenantTypeCode { get; set; } = string.Empty;
    public string TenantTypeDesc { get; set; } = string.Empty;
}
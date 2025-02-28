namespace System.Domain.Features.Tenant;

public class TenantTypeM : AggregateRoot
{
    public int TenantTypeId { get; set; }
    public string TenantTypeCode { get; set; } = string.Empty;
    public string TenantTypeDesc { get; set; } = string.Empty;

    public static TenantTypeM Create(string tenantTypeCode, string tenantTypeDesc)
    {
        TenantTypeM tenantType = new()
        {
            TenantTypeCode = tenantTypeCode,
            TenantTypeDesc = tenantTypeDesc,
        };

        return tenantType;
    }
}
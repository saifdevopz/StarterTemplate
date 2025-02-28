namespace Common.Infrastructure.System;

public class TenantDto
{
    public int TenantId { get; set; }
    public int TenantTypeId { get; set; }
    public int? ParentTenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public DateTime LicenceExpiryDate { get; set; }
}
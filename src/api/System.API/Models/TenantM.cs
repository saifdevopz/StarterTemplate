using System.ComponentModel.DataAnnotations;

namespace System.API.Models;

internal sealed class TenantM
{
    [Key]
    public int TenantId { get; set; }
    public string TenantName { get; private set; } = string.Empty;
    public string DatabaseName { get; private set; } = string.Empty;
    public string ConnectionString { get; private set; } = string.Empty;
    public DateTime LicenceExpiryDate { get; private set; }

    public static TenantM Create(
        string tenantName,
        string databaseName,
        string connectionString)
    {
        TenantM tenant = new()
        {
            TenantName = tenantName,
            DatabaseName = databaseName,
            ConnectionString = connectionString,
            LicenceExpiryDate = DateTime.Now.AddDays(7),
        };

        return tenant;
    }
}


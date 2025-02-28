using System.ComponentModel.DataAnnotations;

namespace Common.Domain.TransferObjects.System;

public class CreateTenantDto
{
    public int TenantTypeId { get; set; }
    public int ParentTenantId { get; set; }

    [Required]
    public string TenantName { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}

public sealed record GetAllTenants
(
    string TenantTypeCode,
    string TenantTypeName
);
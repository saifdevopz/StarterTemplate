namespace Common.Domain.TransferObjects.System;
public sealed record ReadUserDto
(
    int UserId,
    string Email,
    bool IsActive
);

public sealed record WriteUserDto
(
    int UserId,
    int TenantId,
    string Email,
    string Password
);

public sealed record CustomUserClaim
(
    int UserId,
    int TenantId,
    string Email,
    string RoleName,
    string TenantTypeCode,
    string TenantName,
    string DatabaseName
);

public sealed record TokenClaimsResponse
(
    int? UserId = null,
    int? TenantId = null,
    string? Email = null,
    HashSet<string>? RoleName = null,
    string? TenantTypeCode = null,
    string? TenantName = null,
    string? DatabaseName = null,
    string? Expiry = null
);



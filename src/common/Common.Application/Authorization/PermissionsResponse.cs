namespace Common.Application.Authorization;

public sealed record PermissionsResponse(int UserId, HashSet<string> Permissions);

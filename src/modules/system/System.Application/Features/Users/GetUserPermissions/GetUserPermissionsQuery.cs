using Common.Application.Authorization;

namespace System.Application.Features.Users.GetUserPermissions;

public sealed record GetUserPermissionsQuery(int Id) : IQuery<PermissionsResponse>;

using Common.Application.Authorization;
using Common.Domain.Results;
using MediatR;
using System.Application.Features.Users.GetUserPermissions;

namespace System.Infrastructure.Common.Authorization;

internal sealed class PermissionService(ISender sender) : IPermissionService
{
    public async Task<Result<PermissionsResponse>> GetUserPermissionsAsync(int identityId)
    {
        return await sender.Send(new GetUserPermissionsQuery(identityId));
    }
}

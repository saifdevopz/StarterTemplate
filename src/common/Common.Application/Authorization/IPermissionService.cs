using Common.Domain.Results;

namespace Common.Application.Authorization;

public interface IPermissionService
{
    Task<Result<PermissionsResponse>> GetUserPermissionsAsync(int identityId);
}

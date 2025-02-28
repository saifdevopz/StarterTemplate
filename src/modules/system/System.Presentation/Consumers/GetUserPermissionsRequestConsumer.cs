using Common.Application.Authorization;
using MassTransit;
using System.IntegrationEvents.Events;

namespace System.Presentation.Consumers;

public sealed class GetUserPermissionsRequestConsumer(IPermissionService permissionService)
        : IConsumer<GetUserPermissionsRequest>
{
    public async Task Consume(ConsumeContext<GetUserPermissionsRequest> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Result<PermissionsResponse> result =
                await permissionService.GetUserPermissionsAsync(context.Message.IdentityId);

        if (result.IsSuccess)
        {
            await context.RespondAsync(result.Value);
        }
        else
        {
            await context.RespondAsync(result.Error);
        }
    }
}

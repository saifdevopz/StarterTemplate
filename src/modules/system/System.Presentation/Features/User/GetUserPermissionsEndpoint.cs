using Common.Application.Authorization;
using MediatR;
using System.Application.Features.Users.GetUserPermissions;
using System.Presentation.Common;

namespace System.Presentation.Features.User;

internal sealed class GetUserPermissionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("user/permissions", async (int request, ISender sender) =>
        {
            Result<PermissionsResponse> result = await sender.Send(new GetUserPermissionsQuery(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization("inventory:view")
        .WithTags(Tags.User);
    }
}
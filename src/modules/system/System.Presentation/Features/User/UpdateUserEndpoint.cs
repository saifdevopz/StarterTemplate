using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.Users.UpdateUser;
using System.Presentation.Common;

namespace System.Presentation.Features.User;

internal sealed class UpdateTenantTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("user", async (WriteUserDto request, ISender sender) =>
        {
            Result<bool> result = await sender.Send(new UpdateUserCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.User);
    }
}
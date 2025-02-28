using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.Users.GetAllUser;
using System.Presentation.Common;

namespace System.Presentation.Features.User;

internal sealed class GetAllUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("user", async (ISender sender) =>
        {
            Result<List<ReadUserDto>> result = await sender.Send(new GetAllUserQuery());

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.User);
    }
}
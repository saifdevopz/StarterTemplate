using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.Users.GetByIdUser;
using System.Presentation.Common;

namespace System.Presentation.Features.User;

internal sealed class GetByIdUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("user/{request}", async (int request, ISender sender) =>
        {
            Result<ReadUserDto> result = await sender.Send(new GetByIdUserQuery(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.User);
    }
}
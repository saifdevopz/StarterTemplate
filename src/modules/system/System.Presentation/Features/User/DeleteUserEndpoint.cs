using MediatR;
using System.Application.Features.Users.DeleteUser;
using System.Presentation.Common;

namespace System.Presentation.Features.User;

internal sealed class DeleteUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("user/{id}", async (int request, ISender sender) =>
        {
            Result<bool> result = await sender.Send(new DeleteUserCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.User);
    }
}
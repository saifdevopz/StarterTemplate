using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.Authentication.RefreshToken;
using System.Presentation.Common;

namespace System.Presentation.Features.Authentication;

internal sealed class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("token/refreshtoken", async (RefreshTokenRequest request, ISender sender) =>
        {
            Result<TokenResponse> result = await sender.Send(new RefreshTokenCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Token);
    }

}
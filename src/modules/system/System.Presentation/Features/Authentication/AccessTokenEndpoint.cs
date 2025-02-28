using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.Authentication.AccessToken;
using System.Presentation.Common;

namespace System.Presentation.Features.Authentication;

internal sealed class AccessTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("token/accesstoken", async (AccessTokenRequest request, ISender sender) =>
        {
            Result<TokenResponse> result = await sender.Send(new AccessTokenCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Token);
    }

}
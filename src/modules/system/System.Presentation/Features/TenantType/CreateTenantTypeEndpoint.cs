using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.TenantType.CreateTenantType;
using System.Presentation.Common;

namespace System.Presentation.Features.TenantType;

internal sealed class CreateTenantTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("tenanttype/create", async (WriteTenantTypeDto request, ISender sender) =>
        {
            Result<bool> result = await sender.Send(new CreateTenantTypeCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.TenantType);
    }
}
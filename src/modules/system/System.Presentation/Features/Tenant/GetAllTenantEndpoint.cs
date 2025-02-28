using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.Tenant.GetAllTenant;
using System.Presentation.Common;

namespace System.Presentation.Features.Tenant;

internal sealed class GetAllTenantTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("tenant/all", async (ISender sender) =>
        {
            Result<List<GetAllTenants>> result = await sender.Send(new GetAllTenantQuery());

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Tenant);
    }
}

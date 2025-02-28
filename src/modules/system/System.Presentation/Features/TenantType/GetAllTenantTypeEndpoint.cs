using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.TenantType.GetAllTenantType;
using System.Presentation.Common;

namespace System.Presentation.Features.TenantType;

internal sealed class GetAllTenantTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("tenanttype/all", async (ISender sender) =>
        {
            Result<List<ReadTenantTypeDto>> result = await sender.Send(new GetAllTenantTypeQuery());

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.TenantType);
    }
}

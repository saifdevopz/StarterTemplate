using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.TenantType.GetByIdTenantType;
using System.Presentation.Common;

namespace System.Presentation.Features.TenantType;

internal sealed class GetByIdTenantTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("tenanttype/{id}", async (int tenantId, ISender sender) =>
        {
            Result<ReadTenantTypeDto> result = await sender.Send(new GetByIdTenantTypeQuery(tenantId));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.TenantType);
    }
}
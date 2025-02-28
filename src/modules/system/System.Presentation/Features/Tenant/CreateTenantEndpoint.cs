using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.Tenant.CreateTenant;
using System.Domain.Features.Tenant;
using System.Presentation.Common;

namespace System.Presentation.Features.Tenant;

internal sealed class CreateTenantEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("tenant/create", async (CreateTenantDto request, ISender sender) =>
        {
            Result<TenantM> result = await sender.Send(new CreateTenantCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Tenant);
    }
}
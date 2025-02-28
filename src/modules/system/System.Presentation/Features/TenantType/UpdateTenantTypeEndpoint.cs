using Common.Domain.TransferObjects.System;
using MediatR;
using System.Application.Features.TenantType.UpdateTenantType;
using System.Presentation.Common;

namespace System.Presentation.Features.TenantType;

internal sealed class UpdateTenantTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("tenanttype", async (WriteTenantTypeDto request, ISender sender) =>
        {
            Result<bool> result = await sender.Send(new UpdateTenantTypeCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.TenantType);
    }
}
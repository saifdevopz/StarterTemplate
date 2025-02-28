using MediatR;
using System.Application.Features.TenantType.DeleteTenantType;
using System.Presentation.Common;

namespace System.Presentation.Features.TenantType;

internal sealed class DeleteTenantTypeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("tenanttype/{id}", async (int tenantId, ISender sender) =>
        {
            Result<bool> result = await sender.Send(new DeleteTenantTypeCommand(tenantId));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.TenantType);
    }
}
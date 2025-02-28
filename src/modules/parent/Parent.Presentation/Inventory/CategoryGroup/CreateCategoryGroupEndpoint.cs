using Common.Domain.TransferObjects.Inventory;
using MediatR;
using Parent.Application.Inventory.CategoryGroup.CreateCategoryGroup;
using Parent.Domain.Inventory.CategoryGroup;

namespace Parent.Presentation.Inventory.CategoryGroup;

internal sealed class CreateCategoryGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("inventory", async (WriteCategoryGroup request, ISender sender) =>
        {
            Result<CategoryGroupM> result = await sender.Send(new CreateCategoryGroupCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        });
    }
}
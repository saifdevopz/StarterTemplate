using Common.Domain.TransferObjects.Order;
using MediatR;
using Store.Application.Order.CreateOrder;
using Store.Domain.Order;

namespace Store.Presentation.Features.Order;

internal sealed class CreateOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("order", async (WriteOrder request, ISender sender) =>
        {
            Result<OrderM> result = await sender.Send(new CreateOrderCommand(request));

            return result.Match(Results.Ok, ApiResults.Problem);
        });
    }
}
using Common.Domain.Abstractions;

namespace Common.Domain.Features.Order;

public abstract class ProductBase : AggregateRoot
{
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductDesc { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
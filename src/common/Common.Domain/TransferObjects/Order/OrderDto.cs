namespace Common.Domain.TransferObjects.Order;

public sealed record WriteOrder
(
    string OrderCode,
    string OrderDesc
);
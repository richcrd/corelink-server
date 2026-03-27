namespace Corelink.Application.Contracts.Orders;

public sealed record OrderDetailResponse(
    long OrderId,
    decimal Total,
    string Status,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemResponse> Items
);

public sealed record OrderItemResponse(
    string ProductName,
    string? ImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal
);

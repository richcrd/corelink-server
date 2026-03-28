namespace Corelink.Application.Contracts.Orders;

public sealed record OrderSummaryResponse(
    long OrderId,
    decimal Total,
    string Status,
    DateTime CreatedAt,
    string CustomerName,
    string? Phone,
    string? Address,
    string BranchName,
    string? PaymentMethod,
    IReadOnlyList<OrderItemResponse> Products
);

public sealed record OrderSummaryHeaderDto(
    long OrderId, 
    decimal Total, 
    string Status, 
    DateTime CreatedAt, 
    string CustomerName, 
    string? Phone, 
    string? Address, 
    string BranchName, 
    string? PaymentMethod);

public sealed record OrderItemWithIdDto(
    long OrderId,
    string ProductName,
    string? ImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);
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
    string? PaymentMethod
);
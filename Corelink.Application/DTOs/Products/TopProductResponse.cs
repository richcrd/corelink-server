namespace Corelink.Application.Contracts.Products;

public sealed record TopProductResponse(
    long ProductId,
    string ProductName,
    string? ImageUrl,
    long TotalSold
);
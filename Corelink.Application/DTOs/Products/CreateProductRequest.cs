namespace Corelink.Application.Contracts.Products;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    long CategoryId,
    long BranchId,
    decimal Price,
    int Stock
    );
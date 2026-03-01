namespace Corelink.Application.Contracts.Products;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    Guid CategoryId
    );
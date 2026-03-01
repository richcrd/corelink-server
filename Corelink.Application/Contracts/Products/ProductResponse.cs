namespace Corelink.Application.Contracts.Products;

public record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid CategoryId,
    string Status,
    IReadOnlyList<ProductImageResponse> Images,
    IReadOnlyList<ProductBranchResponse> Branches
    );
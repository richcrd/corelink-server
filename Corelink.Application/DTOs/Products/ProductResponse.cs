namespace Corelink.Application.Contracts.Products;

public record ProductResponse(
    long Id,
    string Name,
    string? Description,
    long CategoryId,
    string Status,
    IReadOnlyList<ProductImageResponse> Images,
    IReadOnlyList<ProductBranchResponse> Branches
    );
namespace Corelink.Application.Contracts.ProductCategory;

public record ProductCategoryListResponse(
    long Id,
    string Name,
    string Description,
    string? ImageUrl);
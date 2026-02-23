namespace Corelink.Application.Contracts.ProductCategory;

public record ProductCategoryListResponse(
    Guid Id,
    string Name,
    string Description,
    string? ImageUrl);
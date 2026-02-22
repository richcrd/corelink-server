namespace Corelink.Application.Contracts.ProductCategory;

public record ProductCategoryResponse(
    Guid Id,
    string Name,
    string Description);
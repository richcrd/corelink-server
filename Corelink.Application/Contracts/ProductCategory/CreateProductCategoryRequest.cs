namespace Corelink.Application.Contracts.ProductCategory;

public record CreateProductCategoryRequest(
    string Name,
    string Description);
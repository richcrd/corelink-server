namespace Corelink.Application.Contracts.ProductCategory;

public sealed record PatchProductCategoryRequest(
    string? Name,
    string? Description
);
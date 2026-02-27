using Corelink.Application.Contracts;
using Corelink.Application.Contracts.ProductCategory;

namespace Corelink.Application.Abstractions.Services;

public interface IProductCategoryService
{ 
    Task<Answer<ProductCategoryResponse>> CreateAsync(CreateProductCategoryRequest command);
    Task<Answer<ProductCategoryResponse?>> GetByIdAsync(Guid id);
    Task<Answer<IReadOnlyList<ProductCategoryListResponse>>> GetAllAsync();
    Task<Answer<ProductCategoryResponse>> UpdateAsync(Guid id, PatchProductCategoryRequest command);
    Task<Answer<string>> UpdateImageAsync(
        Guid categoryId,
        Stream imageStream,
        string fileName,
        string contentType);
}
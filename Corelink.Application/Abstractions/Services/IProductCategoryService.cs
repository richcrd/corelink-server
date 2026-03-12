using Corelink.Application.Contracts;
using Corelink.Application.Contracts.ProductCategory;

namespace Corelink.Application.Abstractions.Services;

public interface IProductCategoryService
{ 
    Task<Answer<ProductCategoryResponse>> CreateAsync(CreateProductCategoryRequest command);
    Task<Answer<ProductCategoryResponse?>> GetByIdAsync(long id);
    Task<Answer<IReadOnlyList<ProductCategoryListResponse>>> GetAllAsync();
    Task<Answer<ProductCategoryResponse>> UpdateAsync(long id, PatchProductCategoryRequest command);
    Task<Answer<string>> UpdateImageAsync(
        long categoryId,
        Stream imageStream,
        string fileName,
        string contentType);
}
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.ProductCategory;

namespace Corelink.Application.Abstractions.Services;

public interface IProductCategoryService
{
    Task<Answer<ProductCategoryResponse>> CreateAsync(CreateProductCategoryRequest dto);
    Task<Answer<ProductCategoryResponse?>> GetByIdAsync(Guid id);
    Task<Answer<IReadOnlyList<ProductCategoryResponse>>> GetAllAsync();
}
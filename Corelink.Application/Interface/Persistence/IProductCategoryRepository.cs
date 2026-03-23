using Corelink.Application.Contracts.ProductCategory;
using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IProductCategoryRepository
{
    Task<long> CreateAsync(ProductCategory productCategory);
    Task<ProductCategory?> GetById(long id);
    Task<IReadOnlyList<ProductCategoryListResponse>> GetAllAsync();
    Task AddImageAsync(long categoryId, string imageUrl);
    Task<bool> UpdateAsync(ProductCategory productCategory);
    Task UpdateImageAsync(long categoryId, long imageId);
    Task<string?> GetMainImageUrlAsync(long categoryId);
    Task<long> CreateImageAsync(string url);
}
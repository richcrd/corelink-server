using Corelink.Application.Contracts.ProductCategory;
using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IProductCategoryRepository
{
    Task<Guid> CreateAsync(ProductCategory productCategory);
    Task<ProductCategory?> GetById(Guid id);
    Task<IReadOnlyList<ProductCategoryListResponse>> GetAllAsync();
    Task AddImageAsync(Guid categoryId, string imageUrl);
    Task<bool> UpdateAsync(ProductCategory productCategory);
    Task UpdateImageAsync(Guid categoryId, Guid imageId);
    Task<string?> GetMainImageUrlAsync(Guid categoryId);
    Task<Guid> CreateImageAsync(string url);
}
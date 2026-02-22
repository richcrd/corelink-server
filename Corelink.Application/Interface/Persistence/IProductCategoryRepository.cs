using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IProductCategoryRepository
{
    Task<Guid> CreateAsync(ProductCategory productCategory);
    Task<ProductCategory?> GetById(Guid id);
    Task<IReadOnlyList<ProductCategory>> GetAllAsync();
}
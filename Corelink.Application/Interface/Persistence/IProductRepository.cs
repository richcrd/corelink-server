using Corelink.Application.Contracts.Products;
using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IProductRepository
{
    Task<long> CreateAsync(Product product);
    Task<Product?> GetByIdAsync(long id);
    Task<bool> UpdateAsync(Product product);
    Task<IReadOnlyList<ProductListResponse>> GetByBranchAsync(long branchId);
    Task<bool> AddToBranchAsync(long productId, long branchId, decimal price);
    Task<bool> AddOfferAsync(long productBranchId, decimal offerPrice, DateTime? startDate, DateTime? endDate);
    Task AddImageAsync(long productId, string imageUrl);
    Task<string?> GetMainImageUrlAsync(long productId);
}
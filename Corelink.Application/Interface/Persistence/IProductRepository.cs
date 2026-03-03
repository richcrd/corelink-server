using Corelink.Application.Contracts.Products;
using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IProductRepository
{
    Task<Guid> CreateAsync(Product product);
    Task<Product?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(Product product);
    Task<IReadOnlyList<ProductListResponse>> GetByBranchAsync(Guid branchId);
    Task<bool> AddToBranchAsync(Guid productId, Guid branchId, decimal price);
    Task<bool> AddOfferAsync(Guid productBranchId, decimal offerPrice, DateTime? startDate, DateTime? endDate);
    Task AddImageAsync(Guid productId, string imageUrl);
    Task<string?> GetMainImageUrlAsync(Guid productId);
}
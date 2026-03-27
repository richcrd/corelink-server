using Corelink.Application.Contracts.Products;
using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IProductRepository
{
    Task<long> CreateAsync(Product product);
    Task<Product?> GetByIdAsync(long id);
    Task<bool> UpdateAsync(Product product);
    Task<IReadOnlyList<ProductListResponse>> GetByBranchAsync(long branchId);
    Task<bool> AddToBranchAsync(long productId, long branchId, decimal price, int stock);
    Task<bool> UpdateBranchProductAsync(long productId, long branchId, decimal? price, int? stock);
    Task<bool> AddOfferAsync(long productBranchId, decimal offerPrice, DateTime? startDate, DateTime? endDate);
    Task<bool> AddOrReplaceImageAsync(long productId, string imageUrl);
    Task<string?> GetMainImageUrlAsync(long productId);
    Task<(IReadOnlyList<ProductListResponse> Items, int TotalCount)> GetProductsByCategoryAndBranchAsync(long categoryId, long branchId, int page, int pageSize);
    Task<IReadOnlyList<TopProductResponse>> GetTopProductsByBranchAsync(long branchId, int limit = 5);
    Task<IReadOnlyList<TopProductWithPriceResponse>> GetTopProductsWithPriceByBranchAsync(long branchId, int limit = 5);
}
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Products;

namespace Corelink.Application.Abstractions.Services;

public interface IProductService
{
    Task<Answer<ProductResponse>> CreateAsync(CreateProductRequest request);
    Task<Answer<ProductResponse?>> GetByIdAsync(long id);
    Task<Answer<IReadOnlyList<ProductListResponse>>> GetByBranchAsync(long branchId);
    Task<Answer<ProductResponse>> UpdateAsync(long id, PatchProductRequest request);
    Task<AnswerBase> AddToBranchAsync(long productId, AddProductToBranchRequest request);
    Task<AnswerBase> AddOfferAsync(long productBranchId, CreateProductOfferRequest request);
    Task<AnswerBase> AddImageAsync(long productId, string imageUrl);
    Task<Answer<string?>> GetMainImageUrlAsync(long productId);
}
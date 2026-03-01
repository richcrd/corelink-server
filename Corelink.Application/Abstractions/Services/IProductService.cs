using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Products;

namespace Corelink.Application.Abstractions.Services;

public interface IProductService
{
    Task<Answer<ProductResponse>> CreateAsync(CreateProductRequest request);
    Task<Answer<ProductResponse?>> GetByIdAsync(Guid id);
    Task<Answer<IReadOnlyList<ProductListResponse>>> GetByBranchAsync(Guid branchId);
    Task<Answer<ProductResponse>> UpdateAsync(Guid id, PatchProductRequest request);
    Task<AnswerBase> AddToBranchAsync(Guid productId, AddProductToBranchRequest request);
    Task<AnswerBase> AddOfferAsync(Guid productBranchId, CreateProductOfferRequest request);
}
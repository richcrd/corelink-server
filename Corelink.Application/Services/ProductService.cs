using Corelink.Application.Abstractions.Services;
using Corelink.Application.Common;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Products;
using Corelink.Application.Interface.Persistence;
using Corelink.Application.Mapper;
using Corelink.Domain.Entities;

namespace Corelink.Application.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    private readonly IProductRepository _repository = repository;
    
    public async Task<Answer<ProductResponse>> CreateAsync(CreateProductRequest request)
    {
        var error = Validation.FirstError(
            Validation.Required<ProductResponse>(request.Name, nameof(request.Name)),
            Validation.RequiredGuid<ProductResponse>(request.CategoryId, nameof(request.CategoryId))
        );

        if (error is not null)
            return error;

        var entity = new Product(
            Validation.Trim(request.Name),
            request.Description,
            request.CategoryId
        );

        var id = await _repository.CreateAsync(entity);
        
        return Answer<ProductResponse>.Ok(ProductMapper.ToResponse(entity));
    }

    public async Task<Answer<ProductResponse?>> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return Answer<ProductResponse?>.BadRequest("Id is required");

        var product = await _repository.GetByIdAsync(id);

        return product is null
            ? Answer<ProductResponse?>.NotFound("Product not found")
            : Answer<ProductResponse?>.Ok(ProductMapper.ToResponse(product));
    }

    public async Task<Answer<IReadOnlyList<ProductListResponse>>> GetByBranchAsync(Guid branchId)
    {
        if (branchId == Guid.Empty)
            return Answer<IReadOnlyList<ProductListResponse>>.BadRequest("BranchId is required");

        var products = await _repository.GetByBranchAsync(branchId);
        
        return Answer<IReadOnlyList<ProductListResponse>>.Ok(products);
    }

    public async Task<Answer<ProductResponse>> UpdateAsync(Guid id, PatchProductRequest request)
    {
        if (id == Guid.Empty)
            return Answer<ProductResponse>.BadRequest("Id is required");

        var entity = await _repository.GetByIdAsync(id);
        
        if (entity is null)
            return Answer<ProductResponse>.NotFound("Product not found");
        
        if (request.Name is null && request.Description is null)
            return Answer<ProductResponse>.BadRequest("At least one field must be provided");

        if (request.Name is not null)
            entity.UpdateName(Validation.Trim(request.Name));
        
        if (request.Description is not null)
            entity.UpdateDescription(Validation.Trim(request.Description));

        var updated = await _repository.UpdateAsync(entity);

        return !updated
            ? Answer<ProductResponse>.Error("Update failed")
            : Answer<ProductResponse>.Ok(ProductMapper.ToResponse(entity));
    }

    public async Task<AnswerBase> AddToBranchAsync(Guid productId, AddProductToBranchRequest request)
    {
        if (productId == Guid.Empty)
            return AnswerBase.BadRequest("ProductId is required");
        
        if (request.Price < 0)
            return AnswerBase.BadRequest("Price cannot be negative");

        var added = await _repository.AddToBranchAsync(productId, request.BranchId, request.Price);

        return !added
            ? AnswerBase.Error("Could not add product to branch")
            : AnswerBase.Ok();
    }

    public async Task<AnswerBase> AddOfferAsync(Guid productBranchId, CreateProductOfferRequest request)
    {
        if (productBranchId == Guid.Empty)
            return AnswerBase.BadRequest("ProductBranchId is required");
        
        if (request.OfferPrice < 0)
            return AnswerBase.BadRequest("Offer price invalid");

        var added = await _repository.AddOfferAsync(productBranchId, request.OfferPrice, request.StartDate,
            request.EndDate);

        return !added
            ? AnswerBase.Error("Could not create offer")
            : AnswerBase.Ok();
    }
}
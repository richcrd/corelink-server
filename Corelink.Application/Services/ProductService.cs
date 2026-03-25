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
            Validation.RequiredLong<ProductResponse>(request.CategoryId, nameof(request.CategoryId))
        );

        if (error is not null)
            return error;

        var entity = new Product(
            Validation.Trim(request.Name),
            request.Description,
            request.CategoryId
        );

        var id = await _repository.CreateAsync(entity);
        entity.Id = id;

        // Associate to branch
        await _repository.AddToBranchAsync(id, request.BranchId, request.Price, request.Stock);

        return Answer<ProductResponse>.Ok(ProductMapper.ToResponse(entity));
    }

    public async Task<Answer<ProductResponse?>> GetByIdAsync(long id)
    {
        if (id <= 0)
            return Answer<ProductResponse?>.BadRequest("Id is required");

        var product = await _repository.GetByIdAsync(id);

        return product is null
            ? Answer<ProductResponse?>.NotFound("Product not found")
            : Answer<ProductResponse?>.Ok(ProductMapper.ToResponse(product));
    }

    public async Task<Answer<IReadOnlyList<ProductListResponse>>> GetByBranchAsync(long branchId)
    {
        if (branchId <= 0)
            return Answer<IReadOnlyList<ProductListResponse>>.BadRequest("BranchId is required");

        var products = await _repository.GetByBranchAsync(branchId);
        
        return Answer<IReadOnlyList<ProductListResponse>>.Ok(products);
    }

    public async Task<Answer<ProductResponse>> UpdateAsync(long id, PatchProductRequest request)
    {
        if (id <= 0)
            return Answer<ProductResponse>.BadRequest("Id is required");

        var entity = await _repository.GetByIdAsync(id);
        
        if (entity is null)
            return Answer<ProductResponse>.NotFound("Product not found");
        
        if (request.Name is null && request.Description is null && request.Price is null && request.Stock is null)
            return Answer<ProductResponse>.BadRequest("At least one field must be provided");

        if (request.Name is not null)
            entity.UpdateName(Validation.Trim(request.Name));
        
        if (request.Description is not null)
            entity.UpdateDescription(Validation.Trim(request.Description));

        var updated = await _repository.UpdateAsync(entity);

        if (!updated)
            return Answer<ProductResponse>.Error("Update failed");

        if (request.BranchId.HasValue && (request.Price.HasValue || request.Stock.HasValue))
        {
            await _repository.UpdateBranchProductAsync(id, request.BranchId.Value, request.Price, request.Stock);
        }

        return Answer<ProductResponse>.Ok(ProductMapper.ToResponse(entity));
    }

    public async Task<AnswerBase> AddToBranchAsync(long productId, AddProductToBranchRequest request)
    {
        if (productId <= 0)
            return AnswerBase.BadRequest("ProductId is required");
        
        if (request.Price < 0)
            return AnswerBase.BadRequest("Price cannot be negative");

        var product = await _repository.GetByIdAsync(productId);
        if (product == null)
            return AnswerBase.NotFound("Product not found");

        var added = await _repository.AddToBranchAsync(productId, request.BranchId, request.Price, 0);

        return !added
            ? AnswerBase.Error("Could not add product to branch")
            : AnswerBase.Ok();
    }

    public async Task<AnswerBase> AddOfferAsync(long productBranchId, CreateProductOfferRequest request)
    {
        if (productBranchId <= 0)
            return AnswerBase.BadRequest("ProductBranchId is required");
        
        if (request.OfferPrice < 0)
            return AnswerBase.BadRequest("Offer price invalid");

        var added = await _repository.AddOfferAsync(productBranchId, request.OfferPrice, request.StartDate,
            request.EndDate);

        return !added
            ? AnswerBase.Error("Could not create offer")
            : AnswerBase.Ok();
    }

    public async Task<AnswerBase> AddImageAsync(long productId, string imageUrl)
    {
        if (productId <= 0)
            return AnswerBase.BadRequest("Product id is required");
        
        if (string.IsNullOrWhiteSpace(imageUrl))
            return AnswerBase.BadRequest("Image is required");
        
        var product = await _repository.GetByIdAsync(productId);
        
        if (product is null)
            return AnswerBase.NotFound("Product not found");

        await _repository.AddImageAsync(productId, imageUrl);
        
        return AnswerBase.Ok();
    }

    public async Task<Answer<string?>> GetMainImageUrlAsync(long productId)
    {
        if (productId <= 0)
            return Answer<string?>.BadRequest("ProductId is required");

        var url = await _repository.GetMainImageUrlAsync(productId);
        return Answer<string?>.Ok(url);
    }

    public async Task<Answer<PagedResponse<ProductListResponse>>> GetProductsByCategoryAndBranchAsync(long categoryId, long branchId, int page, int pageSize)
    {
        if (categoryId <= 0)
            return Answer<PagedResponse<ProductListResponse>>.BadRequest("CategoryId is required");
        
        if (branchId <= 0)
            return Answer<PagedResponse<ProductListResponse>>.BadRequest("BranchId is required");
            
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Hard limit

        var (items, totalCount) = await _repository.GetProductsByCategoryAndBranchAsync(categoryId, branchId, page, pageSize);
        
        var pagedResponse = new PagedResponse<ProductListResponse>(items, totalCount, page, pageSize);
        return Answer<PagedResponse<ProductListResponse>>.Ok(pagedResponse);
    }
}
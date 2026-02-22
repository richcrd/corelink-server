using Corelink.Application.Abstractions.Services;
using Corelink.Application.Common;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.ProductCategory;
using Corelink.Application.Interface.Persistence;
using Corelink.Application.Mapper;
using Microsoft.VisualBasic.CompilerServices;

namespace Corelink.Application.Services;

public class ProductCategoryService(IProductCategoryRepository repository) : IProductCategoryService
{
    private static string NormalizeName(string name) => Validation.Trim(name);
    
    public async Task<Answer<ProductCategoryResponse>> CreateAsync(CreateProductCategoryRequest dto)
    {
        var error = Validation.FirstError(
            Validation.Required<ProductCategoryResponse>(dto.Name, nameof(dto.Name))
        );

        if (error is not null) return error;

        var name = NormalizeName(dto.Name);

        var entity = ProductCategoryMapper.ToEntity(dto);
        entity.Name = name;
        
        await repository.CreateAsync(entity);
        
        var response = ProductCategoryMapper.ToResponse(entity);
        return Answer<ProductCategoryResponse>.Ok(response);

    }

    public async Task<Answer<ProductCategoryResponse?>> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return Answer<ProductCategoryResponse?>.BadRequest("Id is required");
        }

        var productCategory = await repository.GetById(id);
        return productCategory is null
            ? Answer<ProductCategoryResponse?>.NotFound("Product Category not found")
            : Answer<ProductCategoryResponse?>.Ok(ProductCategoryMapper.ToResponse(productCategory));
    }

    public async Task<Answer<IReadOnlyList<ProductCategoryResponse>>> GetAllAsync()
    {
        var entities = await repository.GetAllAsync();
        
        if (entities.Count == 0)
            return Answer<IReadOnlyList<ProductCategoryResponse>>.Ok([]);

        var response = entities
            .Select(ProductCategoryMapper.ToResponse)
            .ToList();
            
        return Answer<IReadOnlyList<ProductCategoryResponse>>.Ok(response);
    }
}
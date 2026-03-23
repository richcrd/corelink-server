using Corelink.Application.Contracts.ProductCategory;
using Corelink.Domain.Entities;

namespace Corelink.Application.Mapper;

public static class ProductCategoryMapper
{
    public static ProductCategoryResponse ToResponse(ProductCategory entity)
        => new(
            entity.Id,
            entity.Name,
            entity.Description
        );
    
    public static ProductCategory ToEntity(CreateProductCategoryRequest dto)
        => new()
        {
            Name = dto.Name,
            Description = dto.Description
        };
    
    // public static void UpdateEntity(
    //     ProductCategory entity,
    //     UpdateProductCategoryRequest dto)
    // {
    //     entity.Name = dto.Name;
    //     entity.Description = dto.Description;
    // }
}
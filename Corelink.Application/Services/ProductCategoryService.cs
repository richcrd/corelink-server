using Corelink.Application.Abstractions.Services;
using Corelink.Application.Common;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.ProductCategory;
using Corelink.Application.Contracts.Storage;
using Corelink.Application.Interface.Persistence;
using Corelink.Application.Mapper;
using Corelink.Domain.Entities;
using Microsoft.VisualBasic.CompilerServices;

namespace Corelink.Application.Services;

public class ProductCategoryService(
    IProductCategoryRepository repository, 
    IFileService fileService) 
    : IProductCategoryService
{
    private readonly IProductCategoryRepository _repository = repository;
    private readonly IFileService _fileService = fileService;
    private static string NormalizeName(string name) => Validation.Trim(name);
    
    public async Task<Answer<ProductCategoryResponse>> CreateAsync(CreateProductCategoryRequest command)
    {
        var error = Validation.FirstError(
            Validation.Required<ProductCategoryResponse>(command.Name, nameof(command.Name))
        );

        if (error is not null) return error;

        string? imageUrl = null;
        try
        {
            imageUrl = await fileService.UploadAsync(
                command.ImageStream,
                command.FileName,
                command.ContentType);
            
            var entity = new ProductCategory()
            {
                Name = Validation.Trim(command.Name),
                Description = command.Description
            };
            
            var id = await _repository.CreateAsync(entity);
            await _repository.AddImageAsync(id, imageUrl);
            
            return Answer<ProductCategoryResponse>.Ok(
                new ProductCategoryResponse(
                    id,
                    entity.Name,
                    entity.Description
                ));
        }
        catch
        {
            if (imageUrl is not null)
            {
                await _fileService.DeleteAsync(imageUrl);
            }

            throw;
        }
    }

    public async Task<Answer<ProductCategoryResponse?>> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return Answer<ProductCategoryResponse?>.BadRequest("Id is required");
        }

        var productCategory = await _repository.GetById(id);
        return productCategory is null
            ? Answer<ProductCategoryResponse?>.NotFound("Product Category not found")
            : Answer<ProductCategoryResponse?>.Ok(ProductCategoryMapper.ToResponse(productCategory));
    }

    public async Task<Answer<IReadOnlyList<ProductCategoryListResponse>>> GetAllAsync()
    {
        var response = await _repository.GetAllAsync();
        return Answer<IReadOnlyList<ProductCategoryListResponse>>.Ok(response);
    }
}
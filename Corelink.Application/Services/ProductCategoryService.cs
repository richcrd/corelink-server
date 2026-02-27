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

    public async Task<Answer<ProductCategoryResponse>> UpdateAsync(Guid id, PatchProductCategoryRequest command)
    {
        if (id == Guid.Empty)
            return Answer<ProductCategoryResponse>.BadRequest("Id is required");

        var entity = await _repository.GetById(id);
        
        if (entity is null)
            return Answer<ProductCategoryResponse>.NotFound("Product Category not found");
        
        if (command.Name is null && command.Description is null)
            return Answer<ProductCategoryResponse>
                .BadRequest("At least one field must be provided");

        if (command.Name is not null)
            entity.Name = Validation.Trim(command.Name);
        if (command.Description is not null)
            entity.Description = command.Description;

        var updated = await _repository.UpdateAsync(entity);

        return !updated 
            ? Answer<ProductCategoryResponse>.Error("Update failed") 
            : Answer<ProductCategoryResponse>.Ok(ProductCategoryMapper.ToResponse(entity));
    }

    public async Task<Answer<string>> UpdateImageAsync(Guid categoryId, Stream imageStream, string fileName, string contentType)
    {
        if (categoryId == Guid.Empty)
            return Answer<string>.BadRequest("Id is required");

        var category = await _repository.GetById(categoryId);
        
        if (category is null)
            return Answer<string>.NotFound("Product category not found");
        
        string? oldImageUrl = null;
        string? newImageUrl = null;
        try
        {
            oldImageUrl = await _repository.GetMainImageUrlAsync(categoryId);

            newImageUrl = await _fileService.UploadAsync(imageStream, fileName, contentType);

            var imageId = await _repository.CreateImageAsync(newImageUrl);
            await _repository.UpdateImageAsync(categoryId, imageId);

            if (oldImageUrl is not null)
            {
                await _fileService.DeleteAsync(oldImageUrl);
            }

            return Answer<string>.Ok(newImageUrl);
        }
        catch
        {
            if (newImageUrl is not null)
            {
                await _fileService.DeleteAsync(newImageUrl);
            }

            throw;
        }
    }
    
}
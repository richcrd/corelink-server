using System.Reflection.Metadata;
using corelink_server.Common;
using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.ProductCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreateProductCategoryModel = corelink_server.Models.CreateProductCategoryModel;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("service/product-category")]
public sealed class ProductCategoryController(IProductCategoryService service) : ResponseBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResponse(await service.GetAllAsync());
    }

    [Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateProductCategoryModel request)
    {
        using var stream = request.Image.OpenReadStream();

        var command = new CreateProductCategoryRequest(
            request.Name,
            request.Description,
            stream,
            request.Image.FileName,
            request.Image.ContentType
        );

        return HandleResponse(await service.CreateAsync(command));
    }

    [Authorize]
    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Patch(long id, [FromBody] PatchProductCategoryRequest request)
    {
        return HandleResponse(await service.UpdateAsync(id, request));
    }

    [Authorize]
    [HttpPut("{id:long}/image")]
    public async Task<IActionResult> UpdateImage(long id, IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File is required");
        }

        await using var stream = file.OpenReadStream();

        return HandleResponse(await service.UpdateImageAsync(
            id,
            stream,
            file.FileName,
            file.ContentType));
    }
}
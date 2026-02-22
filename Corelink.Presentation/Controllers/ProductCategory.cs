using corelink_server.Common;
using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts.ProductCategory;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("service/product-category")]
public sealed class ProductCategory(IProductCategoryService service) : ResponseBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return HandleResponse(await service.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCategoryRequest request)
    {
        return HandleResponse(await service.CreateAsync(request));
    }
}
using corelink_server.Common;
using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts.Products;
using Corelink.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("service/[controller]")]
public sealed class ProductController(
    IProductService service, 
    UploadImageHandler handler
    ) : ResponseBase
{
    private readonly IProductService _service = service;

    [HttpPost("{id:guid}/image")]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var url = await handler.HandleAsync(
            stream,
            file.FileName,
            file.ContentType);

        return Ok(new { ImageUrl = url });
    }

    [HttpGet("branch/{branchId:guid}")]
    public async Task<IActionResult> GetByBranch(Guid branchId)
    {
        return HandleResponse(await _service.GetByBranchAsync(branchId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return HandleResponse(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        return HandleResponse(await _service.CreateAsync(request));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] PatchProductRequest request)
    {
        return HandleResponse(await _service.UpdateAsync(id, request));
    }

    [HttpPost("{id:guid}/branch")]
    public async Task<IActionResult> AddToBranch(Guid id, [FromBody] AddProductToBranchRequest request)
    {
        return HandleResponse(await _service.AddToBranchAsync(id, request));
    }
    
    [HttpPost("branch/{productBranchId:guid}/offer")]
    public async Task<IActionResult> AddOffer(Guid productBranchId, [FromBody] CreateProductOfferRequest request)
    {
        return HandleResponse(await service.AddOfferAsync(productBranchId, request));
    }
}
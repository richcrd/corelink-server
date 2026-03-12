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

    [HttpPost("{id:long}/image")]
    public async Task<IActionResult> UploadImage(long id, IFormFile file)
    {
        if (file is null)
            return BadRequest("File is required");
        
        using var stream = file.OpenReadStream();

        var url = await handler.HandleAsync(
            stream,
            file.FileName,
            file.ContentType);

        var result = await _service.AddImageAsync(id, url);

        return HandleResponse(result);
    }

    [HttpGet("{id:long}/image")]
    public async Task<IActionResult> GetMainImage(long id)
    {
        var result = await _service.GetMainImageUrlAsync(id);
        return HandleResponse(result);
    }

    [HttpGet("branch/{branchId:long}")]
    public async Task<IActionResult> GetByBranch(long branchId)
    {
        return HandleResponse(await _service.GetByBranchAsync(branchId));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        return HandleResponse(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        return HandleResponse(await _service.CreateAsync(request));
    }

    [HttpPatch("{id:long}")]
    public async Task<IActionResult> Patch(long id, [FromBody] PatchProductRequest request)
    {
        return HandleResponse(await _service.UpdateAsync(id, request));
    }

    [HttpPost("{id:long}/branch")]
    public async Task<IActionResult> AddToBranch(long id, [FromBody] AddProductToBranchRequest request)
    {
        return HandleResponse(await _service.AddToBranchAsync(id, request));
    }
    
    [HttpPost("branch/{productBranchId:long}/offer")]
    public async Task<IActionResult> AddOffer(long productBranchId, [FromBody] CreateProductOfferRequest request)
    {
        return HandleResponse(await service.AddOfferAsync(productBranchId, request));
    }
}
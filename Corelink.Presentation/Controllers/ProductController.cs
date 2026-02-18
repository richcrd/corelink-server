using corelink_server.Common;
using Corelink.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ResponseBase
{
    private readonly UploadImageHandler _handler;

    public ProductController(UploadImageHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("{id:guid}/image")]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file)
    {
        using var stream = file.OpenReadStream();

        var url = await _handler.HandleAsync(
            stream,
            file.FileName,
            file.ContentType);

        return Ok(new { ImageUrl = url });
    }
}
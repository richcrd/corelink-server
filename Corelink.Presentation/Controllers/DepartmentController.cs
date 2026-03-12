using corelink_server.Common;
using Corelink.Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("service/[controller]")]
public class DepartmentController(IDepartmentService service) : ResponseBase
{
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        return HandleResponse(await service.GetByIdAsync(id));
    }
}
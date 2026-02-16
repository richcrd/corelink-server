using corelink_server.Common;
using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts.Persons;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("service/[controller]")]
public sealed class PersonController(IPersonService service) : ResponseBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        return HandleResponse(await service.GetByIdAsync(id));
    }

    [HttpGet("by-email")]
    public async Task<ActionResult> GetByEmail([FromQuery] string email)
    {
        return HandleResponse(await service.GetByEmailAsync(email));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest request)
    {
        return HandleResponse(await service.CreateAsync(request));
    }
}

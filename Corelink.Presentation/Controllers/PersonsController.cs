using corelink_server.Common;
using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts.Persons;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("api/persons")]
public sealed class PersonsController(IPersonService service) : ResponseBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        return HandleResponse(await service.GetByIdAsync(id, cancellationToken));
    }

    [HttpGet("by-email")]
    public async Task<ActionResult> GetByEmail([FromQuery] string email, CancellationToken cancellationToken)
    {
        return HandleResponse(await service.GetByEmailAsync(email, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest request, CancellationToken cancellationToken)
    {
        return HandleResponse(await service.CreateAsync(request, cancellationToken));
    }
}

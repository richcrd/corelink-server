using corelink_server.Common;
using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("service/[controller]")]
public sealed class LocationController(ILocationService service) : ResponseBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLocationRequest request)
    {
        return HandleResponse(await service.CreateAsync(request));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return HandleResponse(await service.GetByIdAsync(id));
    }

    [HttpGet("by-department/{departmentId:guid}")]
    public async Task<IActionResult> ListByDepartment(Guid departmentId)
    {
        return HandleResponse(await service.ListByDepartmentIdAsync(departmentId));
    }
}

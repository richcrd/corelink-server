using corelink_server.Common;
using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts.Locations;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("service/[controller]")]
public sealed class BranchController(IBranchService service) : ResponseBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBranchRequest request)
    {
        return HandleResponse(await service.CreateAsync(request));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        return HandleResponse(await service.GetByIdAsync(id));
    }

    [HttpGet("by-department/{departmentId:long}")]
    public async Task<IActionResult> ListByDepartment(long departmentId)
    {
        return HandleResponse(await service.ListByDepartmentIdAsync(departmentId));
    }
}

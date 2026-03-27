using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("service/[controller]")]
public sealed class OrdersController(IOrderService service) : ControllerBase
{
    protected long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public Task<Answer<IReadOnlyList<OrderSummaryResponse>>> GetMyOrders() 
        => service.GetOrdersAsync(UserId);

    [HttpGet("all")]
    public Task<Answer<IReadOnlyList<OrderSummaryResponse>>> GetAllOrders() 
        => service.GetOrdersAsync();

    [HttpGet("{id:long}/details")]
    public Task<Answer<OrderDetailResponse>> GetOrderDetails(long id) 
        => service.GetOrderDetailsAsync(id, UserId);
}

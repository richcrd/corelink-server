using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("service/[controller]")]
public sealed class CartController(ICartService service) : ControllerBase
{
    protected long UserId =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public Task<Cart> Get()
    {
        return service.GetCartAsync(UserId);
    }

    [HttpPost("items")]
    public Task<Cart> AddItem(AddCartItemRequest request)
    {
        return service.AddItemAsync(UserId, request.BranchProductId, request.Quantity);
    }

    [HttpPatch("items/{branchProductId}")]
    public Task<Cart> UpdateItem(long branchProductId, UpdateCartItemRequest request)
    {
        return service.UpdateItemAsync(UserId, branchProductId, request.QuantityDelta);
    }

    [HttpDelete("items/{branchProductId}")]
    public Task<Cart> RemoveItem(long branchProductId)
    {
        return service.RemoveItemAsync(UserId, branchProductId);
    }

    [HttpDelete]
    public Task<Cart> Clear()
    {
        return service.ClearCartAsync(UserId);
    }
}

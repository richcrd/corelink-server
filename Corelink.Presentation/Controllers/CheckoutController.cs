using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Corelink.Application.Abstractions.Services;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("service/[controller]")]
public sealed class CheckoutController(ICheckoutService service) : ControllerBase
{
    protected long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("validate")]
    public Task<Answer<CartValidationResponse>> Validate() => service.ValidateCartAsync(UserId);

    [HttpPost]
    public Task<Answer<CheckoutResponse>> Checkout([FromBody] CheckoutRequest request) => service.ProcessCheckoutAsync(UserId, request);
}

using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("service/[controller]")]
public sealed class PaymentMethodController(IPaymentMethodService service) : ControllerBase
{
    [HttpGet]
    public Task<Answer<IEnumerable<PaymentMethodResponse>>> Get()
    {
        return service.GetActivePaymentMethodsAsync();
    }
}

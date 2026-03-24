using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Checkout;

namespace Corelink.Application.Abstractions.Services;

public interface ICheckoutService
{
    Task<Answer<CartValidationResponse>> ValidateCartAsync(long userId);
    Task<Answer<CheckoutResponse>> ProcessCheckoutAsync(long userId, CheckoutRequest request);
}

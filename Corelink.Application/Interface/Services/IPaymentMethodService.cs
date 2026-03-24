using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Checkout;

namespace Corelink.Application.Abstractions.Services;

public interface IPaymentMethodService
{
    Task<Answer<IEnumerable<PaymentMethodResponse>>> GetActivePaymentMethodsAsync();
}

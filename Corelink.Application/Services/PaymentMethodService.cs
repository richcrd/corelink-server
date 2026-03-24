using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Checkout;
using Corelink.Application.Interface.Persistence;

namespace Corelink.Application.Services;

public sealed class PaymentMethodService(IPaymentMethodRepository repository) : IPaymentMethodService
{
    public async Task<Answer<IEnumerable<PaymentMethodResponse>>> GetActivePaymentMethodsAsync()
    {
        var methods = await repository.GetActivePaymentMethodsAsync();
        return Answer<IEnumerable<PaymentMethodResponse>>.Ok(methods);
    }
}

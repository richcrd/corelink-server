using Corelink.Application.Contracts.Checkout;

namespace Corelink.Application.Interface.Persistence;

public interface IPaymentMethodRepository
{
    Task<IEnumerable<PaymentMethodResponse>> GetActivePaymentMethodsAsync();
}

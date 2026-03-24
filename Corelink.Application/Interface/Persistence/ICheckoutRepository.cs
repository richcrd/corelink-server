using Corelink.Application.Contracts.Checkout;

namespace Corelink.Application.Interface.Persistence;

public interface ICheckoutRepository
{
    Task<string> GetCartStatusAsync(long cartId, long userId);
    Task<IEnumerable<CartCheckoutItem>> GetCartItemsForCheckoutAsync(long cartId);
    Task<long> ProcessCheckoutTransactionAsync(long cartId, long userId, decimal total, IEnumerable<CartCheckoutItem> items, long paymentMethodId, string? paymentReference);
}

using Corelink.Application.Contracts.Cart;

namespace Corelink.Application.Interface.Persistence;

public interface ICartRepository
{
    Task<Cart?> GetCartByUserId(long userId);
    Task<Cart> GetOrCreateCart(long userId, long branchId);
    Task<IReadOnlyList<CartItem>> GetCartItems(long cartId);
    Task<CartItem?> GetCartItem(long cartId, long branchProductId);
    Task AddItem(long cartId, long branchProductId, int quantity);
    Task UpdateQuantity(long cartItemId, int quantity);
    Task RemoveItem(long cartId, long branchProductId);
    Task ClearCart(long cartId);
    Task<BranchProduct?> GetBranchProduct(long branchProductId);
}

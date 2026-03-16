using Corelink.Application.Contracts.Cart;

namespace Corelink.Application.Abstractions.Services;

public interface ICartService
{
    Task<Cart> GetCartAsync(long userId);
    Task<Cart> AddItemAsync(long userId, long branchProductId, int quantity);
    Task<Cart> UpdateItemAsync(long userId, long branchProductId, int quantityDelta);
    Task<Cart> RemoveItemAsync(long userId, long branchProductId);
    Task<Cart> ClearCartAsync(long userId);
}

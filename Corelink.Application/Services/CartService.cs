using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts.Cart;
using Corelink.Application.Interface.Persistence;

namespace Corelink.Application.Services;

public sealed class CartService(ICartRepository repository) : ICartService
{
    public Task<Cart> GetCartAsync(long userId)
    {
        return LoadCart(userId);
    }

    public async Task<Cart> AddItemAsync(long userId, long branchProductId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        var cart = await repository.GetCartByUserId(userId);

        if (cart == null)
        {
            var branchProduct = await repository.GetBranchProduct(branchProductId)
                              ?? throw new KeyNotFoundException($"Branch product not found (id={branchProductId}). Send branch_product.id (not product.id).");

            cart = await repository.GetOrCreateCart(userId, branchProduct.BranchId);
        }
        else
        {
            var branchProduct = await repository.GetBranchProduct(branchProductId)
                              ?? throw new KeyNotFoundException($"Branch product not found (id={branchProductId}). Send branch_product.id (not product.id).");

            if (cart.BranchId != branchProduct.BranchId)
            {
                await repository.ClearCart(cart.Id);
                cart = await repository.GetOrCreateCart(userId, branchProduct.BranchId);
            }
        }

        var existingItem = await repository.GetCartItem(cart.Id, branchProductId);

        if (existingItem == null)
            await repository.AddItem(cart.Id, branchProductId, quantity);
        else
            await repository.UpdateQuantity(existingItem.Id, existingItem.Quantity + quantity);

        return await LoadCart(userId);
    }

    public async Task<Cart> UpdateItemAsync(long userId, long branchProductId, int quantityDelta)
    {
        var cart = await repository.GetCartByUserId(userId);
        if (cart is null)
            throw new KeyNotFoundException("Cart not found");

        var existingItem = await repository.GetCartItem(cart.Id, branchProductId);
        if (existingItem is null)
            throw new KeyNotFoundException("Cart item not found");

        var nextQuantity = existingItem.Quantity + quantityDelta;
        if (nextQuantity <= 0)
            await repository.RemoveItem(cart.Id, branchProductId);
        else
            await repository.UpdateQuantity(existingItem.Id, nextQuantity);

        return await GetCartAsync(userId);
    }

    public async Task<Cart> RemoveItemAsync(long userId, long branchProductId)
    {
        var cart = await repository.GetCartByUserId(userId);
        if (cart != null)
            await repository.RemoveItem(cart.Id, branchProductId);

        return await LoadCart(userId);
    }

    public async Task<Cart> ClearCartAsync(long userId)
    {
        var cart = await repository.GetCartByUserId(userId);
        if (cart != null)
            await repository.ClearCart(cart.Id);

        return await LoadCart(userId);
    }

    private async Task<Cart> LoadCart(long userId)
    {
        var cart = await repository.GetCartByUserId(userId);

        if (cart == null)
        {
            return new Cart { UserId = userId };
        }

        cart.Items = await repository.GetCartItems(cart.Id);

        return cart;
    }
}

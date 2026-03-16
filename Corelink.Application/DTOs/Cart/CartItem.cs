namespace Corelink.Application.Contracts.Cart;

public sealed class CartItem
{
    public long Id { get; init; }
    public long BranchProductId { get; init; }
    public long ProductId { get; init; }
    public string ProductName { get; init; } = default!;
    public string? ImageUrl { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public decimal Subtotal => Math.Round(Price * Quantity, 2);
}

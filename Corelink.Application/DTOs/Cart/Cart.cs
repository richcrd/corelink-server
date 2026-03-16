namespace Corelink.Application.Contracts.Cart;

public sealed class Cart
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public long BranchId { get; init; }
    public IReadOnlyList<CartItem> Items { get; set; } = [];

    public int TotalItems => Items.Sum(x => x.Quantity);
    public decimal TotalAmount => Math.Round(Items.Sum(x => x.Subtotal), 2);
}

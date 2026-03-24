namespace Corelink.Application.Contracts.Checkout;

public sealed class CartCheckoutItem
{
    public long BranchProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public string ProductStatus { get; set; } = string.Empty;
    public string BranchProductStatus { get; set; } = string.Empty;
}

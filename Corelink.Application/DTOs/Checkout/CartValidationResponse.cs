namespace Corelink.Application.Contracts.Checkout;

public sealed class CartValidationResponse
{
    public bool IsValid { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public List<string> Errors { get; set; } = new();
}

namespace Corelink.Application.Contracts.Checkout;

public sealed class PaymentMethodResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
}

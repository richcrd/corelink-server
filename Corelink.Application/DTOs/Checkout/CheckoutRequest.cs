using System.ComponentModel.DataAnnotations;

namespace Corelink.Application.Contracts.Checkout;

public sealed class CheckoutRequest
{
    [Required]
    public long PaymentMethodId { get; set; }
    
    public string? PaymentReference { get; set; }
}

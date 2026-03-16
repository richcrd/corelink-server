namespace Corelink.Application.Contracts.Cart;

public sealed record UpdateCartItemRequest(
    int QuantityDelta
);

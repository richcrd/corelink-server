namespace Corelink.Application.Contracts.Cart;

public sealed record AddCartItemRequest(
    long BranchProductId,
    int Quantity
);

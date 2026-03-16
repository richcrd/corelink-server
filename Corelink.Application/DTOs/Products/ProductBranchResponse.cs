namespace Corelink.Application.Contracts.Products;

public sealed record ProductBranchResponse(
    long Id,
    long BranchId,
    decimal Price,
    decimal FinalPrice,
    string Status,
    IReadOnlyList<ProductOfferResponse> Offers
    );
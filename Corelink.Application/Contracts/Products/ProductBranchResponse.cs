namespace Corelink.Application.Contracts.Products;

public sealed record ProductBranchResponse(
    Guid Id,
    Guid BranchId,
    decimal Price,
    decimal FinalPrice,
    string Status,
    IReadOnlyList<ProductOfferResponse> Offers
    );
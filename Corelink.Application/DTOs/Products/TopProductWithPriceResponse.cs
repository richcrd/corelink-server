namespace Corelink.Application.Contracts.Products;

public sealed record TopProductWithPriceResponse(
    long Id,
    long BranchProductId,
    string Name,
    string? ImageUrl,
    decimal OriginalPrice,
    decimal FinalPrice,
    decimal? OfferPrice,
    bool HasDiscount,
    decimal? DiscountPercentage,
    long TotalSold
);
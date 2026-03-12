namespace Corelink.Application.Contracts.Products;

public sealed class ProductListResponse
{
    public long Id { get; init; }
    public string Name { get; init; } = default!;

    public string? ImageUrl { get; init; }

    public decimal OriginalPrice { get; init; }
    public decimal FinalPrice { get; init; }

    public decimal? OfferPrice { get; init; }

    public bool HasDiscount => 
        OfferPrice.HasValue && OfferPrice.Value < OriginalPrice;

    public decimal? DiscountPercentage =>
        HasDiscount
            ? Math.Round(((OriginalPrice - OfferPrice!.Value) / OriginalPrice) * 100, 2)
            : null;
}
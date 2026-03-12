using Corelink.Domain.Entities.Base;
using Corelink.Domain.Enums;

namespace Corelink.Domain.Entities;

public class ProductBranch : BaseEntity
{
    public long BranchId { get; }
    public decimal Price { get; private set; }
    public StatusEnum Status { get; private set; }

    private readonly List<ProductOffer> _offers = new();
    public IReadOnlyCollection<ProductOffer> Offers => _offers;

    public ProductBranch(
        long id,
        long branchId,
        decimal price,
        StatusEnum status)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative");

        Id = id;
        BranchId = branchId;
        Price = price;
        Status = status;
    }

    public void AddOffer(ProductOffer offer)
    {
        if (offer.OfferPrice >= Price)
            throw new InvalidOperationException("Offer price must be lower than base price");
        
        if (_offers.Any(o => o.IsActive()))
            throw new InvalidOperationException("There is already an active offer");
        
        _offers.Add(offer);
    }

    public decimal GetFinalPrice()
    {
        var activeOffer = _offers.FirstOrDefault(o => o.IsActive());
        return activeOffer?.OfferPrice ?? Price;
    }

    public decimal? GetDiscountPercentage()
    {
        var activeOffer = _offers.FirstOrDefault(o => o.IsActive());
        if (activeOffer is null)
            return null;
        return Math.Round(((Price - activeOffer.OfferPrice) / Price) * 100, 2);
    }
}
using Corelink.Domain.Entities.Base;
using Corelink.Domain.Enums;

namespace Corelink.Domain.Entities;

public class ProductOffer : BaseEntity
{
    public decimal OfferPrice { get; }
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }
    public StatusEnum Status { get; private set; } = StatusEnum.Active;

    public ProductOffer(
        Guid id,
        decimal offerPrice,
        DateTime? startDate,
        DateTime? endDate,
        StatusEnum status)
    {
        if (offerPrice < 0)
            throw new ArgumentException("Offer price cannot be negative");

        Id = id;
        OfferPrice = offerPrice;
        StartDate = startDate;
        EndDate = endDate;
        Status = status;
    }

    public bool IsActive()
    {
        if (Status != StatusEnum.Active)
            return false;

        var now = DateTime.UtcNow;

        if (StartDate.HasValue && StartDate > now)
            return false;

        if (EndDate.HasValue && EndDate < now)
            return false;
        
        return true;
    }
}
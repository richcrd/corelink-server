namespace Corelink.Application.Contracts.Products;

public record ProductOfferResponse(
    long Id,
    decimal OfferPrice,
    DateTime? StartDate,
    DateTime? EndDate,
    string Status,
    bool IsActive
    );
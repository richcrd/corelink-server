namespace Corelink.Application.Contracts.Products;

public record ProductOfferResponse(
    Guid Id,
    decimal OfferPrice,
    DateTime? StartDate,
    DateTime? EndDate,
    string Status,
    bool IsActive
    );
namespace Corelink.Application.Contracts.Products;

public sealed record CreateProductOfferRequest(
    decimal OfferPrice,
    DateTime? StartDate,
    DateTime? EndDate
    );
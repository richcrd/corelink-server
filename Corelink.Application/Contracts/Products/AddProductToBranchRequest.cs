namespace Corelink.Application.Contracts.Products;

public sealed record AddProductToBranchRequest(
    Guid BranchId,
    decimal Price
    );
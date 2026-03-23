namespace Corelink.Application.Contracts.Products;

public sealed record AddProductToBranchRequest(
    long BranchId,
    decimal Price
    );
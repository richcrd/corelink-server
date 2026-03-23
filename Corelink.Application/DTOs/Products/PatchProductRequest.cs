namespace Corelink.Application.Contracts.Products;

public sealed record PatchProductRequest(
    string? Name,
    string? Description
    );
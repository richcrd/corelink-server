namespace Corelink.Application.Contracts.Products;

public sealed record ProductImageResponse(
    long Id,
    string Url,
    bool IsMain,
    int Position
    );
namespace Corelink.Application.Contracts.Products;

public sealed record ProductImageResponse(
    Guid Id,
    string Url,
    bool IsMain,
    int Position
    );
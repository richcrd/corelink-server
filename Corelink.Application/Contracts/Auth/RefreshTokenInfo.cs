namespace Corelink.Application.Contracts.Auth;

public sealed record RefreshTokenInfo(
    Guid Id,
    Guid UserId,
    DateTime ExpiresAt,
    DateTime? RevokedAt);

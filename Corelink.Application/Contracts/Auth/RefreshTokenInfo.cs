namespace Corelink.Application.Contracts.Auth;

public sealed record RefreshTokenInfo(
    long Id,
    long UserId,
    DateTime ExpiresAt,
    DateTime? RevokedAt);

namespace Corelink.Application.Contracts.Auth;

public sealed record RefreshResponse(
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);

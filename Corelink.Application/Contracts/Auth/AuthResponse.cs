namespace Corelink.Application.Contracts.Auth;

public sealed record AuthResponse(
    long UserId,
    long PersonId,
    string Username,
    string Role,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);

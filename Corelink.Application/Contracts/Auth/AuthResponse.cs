namespace Corelink.Application.Contracts.Auth;

public sealed record AuthResponse(
    Guid UserId,
    Guid PersonId,
    string Username,
    string Role,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt);

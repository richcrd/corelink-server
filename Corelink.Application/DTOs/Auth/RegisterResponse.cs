namespace Corelink.Application.Contracts.Auth;

public sealed record RegisterResponse(
    long UserId,
    long PersonId,
    string Username,
    string Role);

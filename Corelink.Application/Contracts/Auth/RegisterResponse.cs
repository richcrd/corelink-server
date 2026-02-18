namespace Corelink.Application.Contracts.Auth;

public sealed record RegisterResponse(
    Guid UserId,
    Guid PersonId,
    string Username,
    string Role);

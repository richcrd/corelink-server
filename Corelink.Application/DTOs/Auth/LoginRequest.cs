namespace Corelink.Application.Contracts.Auth;

public sealed record LoginRequest(
    string Username,
    string Password);

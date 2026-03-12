namespace Corelink.Application.Contracts.Auth;

public sealed record RegisterRequest(
    string Username,
    string Password,
    string FirstName,
    string LastName,
    string Email,
    long BranchId,
    string? PhoneNumber,
    string? Address);

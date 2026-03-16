using Corelink.Domain.Enums;

namespace Corelink.Application.Contracts.Auth;

public sealed record UserAuthInfo(
    long UserId,
    long PersonId,
    string Username,
    string PasswordHash,
    long RoleId,
    string RoleName,
    StatusEnum Status);

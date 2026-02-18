using Corelink.Domain.Enums;

namespace Corelink.Application.Contracts.Auth;

public sealed record UserAuthInfo(
    Guid UserId,
    Guid PersonId,
    string Username,
    string PasswordHash,
    Guid RoleId,
    string RoleName,
    StatusEnum Status);

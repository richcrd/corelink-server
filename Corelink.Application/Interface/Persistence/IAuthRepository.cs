using Corelink.Application.Contracts.Auth;
using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IAuthRepository
{
    Task<UserAuthInfo?> GetByUsernameAsync(string username);
    Task<UserAuthInfo?> GetByUserIdAsync(Guid userId);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);

    Task<Guid?> GetRoleIdByNameAsync(string roleName);

    Task<(Guid PersonId, Guid UserId)> CreatePersonAndUserAsync(
        Person person,
        string username,
        string passwordHash,
        Guid roleId);

    Task<Guid> CreateRefreshTokenAsync(Guid userId, string tokenHash, DateTime expiresAt);
    Task<RefreshTokenInfo?> GetRefreshTokenAsync(string tokenHash);
    Task RevokeRefreshTokenAsync(Guid refreshTokenId, DateTime revokedAt, Guid? replacedByTokenId);
}

using Corelink.Application.Contracts.Auth;
using Corelink.Domain.Entities;

namespace Corelink.Application.Interface.Persistence;

public interface IAuthRepository
{
    Task<UserAuthInfo?> GetByUsernameAsync(string username);
    Task<UserAuthInfo?> GetByUserIdAsync(long userId);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);

    Task<long?> GetRoleIdByNameAsync(string roleName);

    Task<(long PersonId, long UserId)> CreatePersonAndUserAsync(
        Person person,
        string username,
        string passwordHash,
        long roleId);

    Task<long> CreateRefreshTokenAsync(long userId, string tokenHash, DateTime expiresAt);
    Task<RefreshTokenInfo?> GetRefreshTokenAsync(string tokenHash);
    Task RevokeRefreshTokenAsync(long refreshTokenId, DateTime revokedAt, long? replacedByTokenId);
}

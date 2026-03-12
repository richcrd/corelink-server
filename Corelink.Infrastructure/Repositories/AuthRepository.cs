using Corelink.Application.Contracts.Auth;
using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class AuthRepository(IDbConnectionFactory connectionFactory) : IAuthRepository
{
    public async Task<UserAuthInfo?> GetByUsernameAsync(string username)
    {
        const string sql = """
            SELECT
                u.id AS UserId,
                u.person_id AS PersonId,
                u.username AS Username,
                u.password AS PasswordHash,
                u.rol_id AS RoleId,
                r.name AS RoleName,
                u.status::text AS Status
            FROM app_user u
            INNER JOIN catalog_roles r ON r.id = u.rol_id
            WHERE u.username = @Username
            LIMIT 1;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<UserAuthInfo>(sql, new { Username = username });
    }

    public async Task<UserAuthInfo?> GetByUserIdAsync(long userId)
    {
        const string sql = """
            SELECT
                u.id AS UserId,
                u.person_id AS PersonId,
                u.username AS Username,
                u.password AS PasswordHash,
                u.rol_id AS RoleId,
                r.name AS RoleName,
                u.status::text AS Status
            FROM app_user u
            INNER JOIN catalog_roles r ON r.id = u.rol_id
            WHERE u.id = @UserId
            LIMIT 1;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<UserAuthInfo>(sql, new { UserId = userId });
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1
                FROM app_user
                WHERE username = @Username
            );
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(sql, new { Username = username });
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1
                FROM person
                WHERE email = @Email
            );
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(sql, new { Email = email });
    }

    public async Task<long?> GetRoleIdByNameAsync(string roleName)
    {
        const string sql = """
            SELECT id
            FROM catalog_roles
            WHERE LOWER(name) = LOWER(@Name)
            LIMIT 1;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<long?>(sql, new { Name = roleName });
    }

    public async Task<(long PersonId, long UserId)> CreatePersonAndUserAsync(
        Person person,
        string username,
        string passwordHash,
        long roleId)
    {
        const string insertPerson = """
            INSERT INTO person
                (first_name, last_name, email, phone_number, address, branch_id, status)
            VALUES
                (@FirstName, @LastName, @Email, @PhoneNumber, @Address, @BranchId, @Status::status_enum)
            RETURNING id;
            """;

        const string insertUser = """
            INSERT INTO app_user
                (person_id, username, password, rol_id, status)
            VALUES
                (@PersonId, @Username, @PasswordHash, @RoleId, @Status::status_enum)
            RETURNING id;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        await using var tx = await connection.BeginTransactionAsync();

        try
        {
            var personId = await connection.ExecuteScalarAsync<long>(
                insertPerson,
                new
                {
                    person.FirstName,
                    person.LastName,
                    person.Email,
                    person.PhoneNumber,
                    person.Address,
                    person.BranchId,
                    Status = person.Status.ToDb()
                },
                tx);

            var userId = await connection.ExecuteScalarAsync<long>(
                insertUser,
                new
                {
                    PersonId = personId,
                    Username = username,
                    PasswordHash = passwordHash,
                    RoleId = roleId,
                    Status = StatusEnum.ACTIVE.ToDb()
                },
                tx);

            await tx.CommitAsync();
            return (personId, userId);
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<long> CreateRefreshTokenAsync(long userId, string tokenHash, DateTime expiresAt)
    {
        const string sql = """
            INSERT INTO app_user_refresh_token
                (user_id, token_hash, expires_at)
            VALUES
                (@UserId, @TokenHash, @ExpiresAt)
            RETURNING id;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<long>(sql, new
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAt = expiresAt
        });
    }

    public async Task<RefreshTokenInfo?> GetRefreshTokenAsync(string tokenHash)
    {
        const string sql = """
            SELECT
                id,
                user_id AS UserId,
                expires_at AS ExpiresAt,
                revoked_at AS RevokedAt
            FROM app_user_refresh_token
            WHERE token_hash = @TokenHash
            LIMIT 1;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<RefreshTokenInfo>(sql, new { TokenHash = tokenHash });
    }

    public async Task RevokeRefreshTokenAsync(long refreshTokenId, DateTime revokedAt, long? replacedByTokenId)
    {
        const string sql = """
            UPDATE app_user_refresh_token
            SET revoked_at = @RevokedAt,
                replaced_by_token_id = @ReplacedByTokenId
            WHERE id = @Id;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        await connection.ExecuteAsync(sql, new
        {
            Id = refreshTokenId,
            RevokedAt = revokedAt,
            ReplacedByTokenId = replacedByTokenId
        });
    }
}

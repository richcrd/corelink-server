using Corelink.Application.Abstractions.Security;
using Corelink.Application.Abstractions.Services;
using Corelink.Application.Common;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Auth;
using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Corelink.Application.Services;

public sealed class AuthService(
    IAuthRepository repository,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenHasher refreshTokenHasher,
    IPasswordHasher passwordHasher,
    IOptions<AuthOptions> authOptions) : IAuthService
{
    private readonly AuthOptions _authOptions = authOptions.Value;

    private static string NormalizeUsername(string username) => Validation.Trim(username);
    private static string NormalizeEmail(string email) => Validation.Trim(email);

    private static Answer<T>? EnsureActive<T>(UserAuthInfo user)
        => Validation.Ensure<T>(user.Status == StatusEnum.Active, "User is not active");

    private async Task<Answer<AuthResponse>> CreateSessionAsync(UserAuthInfo user, string message)
    {
        var (accessToken, accessExpiresAt) = jwtTokenGenerator.GenerateAccessToken(user);
        var (refreshToken, refreshExpiresAt) = refreshTokenGenerator.GenerateRefreshToken();
        var refreshTokenHash = refreshTokenHasher.Hash(refreshToken);

        await repository.CreateRefreshTokenAsync(user.UserId, refreshTokenHash, refreshExpiresAt);

        return Answer<AuthResponse>.Ok(new AuthResponse(
            user.UserId,
            user.PersonId,
            user.Username,
            user.RoleName,
            accessToken,
            accessExpiresAt,
            refreshToken,
            refreshExpiresAt),
            message);
    }

    public async Task<Answer<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var error = Validation.FirstError(
            Validation.Required<AuthResponse>(request.Username, nameof(request.Username)),
            Validation.Required<AuthResponse>(request.Password, nameof(request.Password)),
            Validation.Required<AuthResponse>(request.FirstName, nameof(request.FirstName)),
            Validation.Required<AuthResponse>(request.LastName, nameof(request.LastName)),
            Validation.Required<AuthResponse>(request.Email, nameof(request.Email)),
            Validation.RequiredGuid<AuthResponse>(request.LocationId, nameof(request.LocationId)));

        if (error is not null) return error;

        var username = NormalizeUsername(request.Username);
        var email = NormalizeEmail(request.Email);

        if (await repository.UsernameExistsAsync(username))
        {
            return Answer<AuthResponse>.BadRequest("Username already exists");
        }

        if (await repository.EmailExistsAsync(email))
        {
            return Answer<AuthResponse>.BadRequest("Email already exists");
        }

        var roleId = await repository.GetRoleIdByNameAsync(_authOptions.DefaultRoleName);
        if (roleId is null)
        {
            return Answer<AuthResponse>.BadRequest($"Default role '{_authOptions.DefaultRoleName}' does not exist");
        }

        var person = new Person
        {
            FirstName = Validation.Trim(request.FirstName!),
            LastName = Validation.Trim(request.LastName!),
            Email = email,
            PhoneNumber = Validation.TrimToNull(request.PhoneNumber),
            Address = Validation.TrimToNull(request.Address),
            LocationId = request.LocationId,
            Status = StatusEnum.Active
        };

        var passwordHash = passwordHasher.Hash(request.Password);

        await repository.CreatePersonAndUserAsync(
            person,
            username,
            passwordHash,
            roleId.Value);

        var userInfo = await repository.GetByUsernameAsync(username);
        if (userInfo is null)
        {
            return Answer<AuthResponse>.Error("User created but cannot be loaded");
        }

        var active = EnsureActive<AuthResponse>(userInfo);
        if (active is not null)
        {
            return active;
        }

        return await CreateSessionAsync(userInfo, "Registered successfully");
    }

    public async Task<Answer<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var error = Validation.FirstError(
            Validation.Required<AuthResponse>(request.Username, nameof(request.Username)),
            Validation.Required<AuthResponse>(request.Password, nameof(request.Password)));

        if (error is not null) return error;

        var username = NormalizeUsername(request.Username);
        var user = await repository.GetByUsernameAsync(username);

        if (user is null)
        {
            return Answer<AuthResponse>.BadRequest("Invalid username or password");
        }

        var active = EnsureActive<AuthResponse>(user);
        if (active is not null)
        {
            return active;
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Answer<AuthResponse>.BadRequest("Invalid username or password");
        }

        return await CreateSessionAsync(user, "Login successful");
    }

    public async Task<Answer<RefreshResponse>> RefreshAsync(RefreshRequest request)
    {
        var error = Validation.Required<RefreshResponse>(request.RefreshToken, nameof(request.RefreshToken));
        if (error is not null) return error;

        var now = DateTime.UtcNow;

        var incomingHash = refreshTokenHasher.Hash(request.RefreshToken!.Trim());
        var existing = await repository.GetRefreshTokenAsync(incomingHash);

        if (existing is null)
        {
            return Answer<RefreshResponse>.BadRequest("Invalid refresh token");
        }

        if (existing.RevokedAt is not null)
        {
            return Answer<RefreshResponse>.BadRequest("Refresh token revoked");
        }

        if (existing.ExpiresAt <= now)
        {
            return Answer<RefreshResponse>.BadRequest("Refresh token expired");
        }

        // Rotate
        var (newRefreshToken, newRefreshExpiresAt) = refreshTokenGenerator.GenerateRefreshToken();
        var newRefreshHash = refreshTokenHasher.Hash(newRefreshToken);
        var newRefreshId = await repository.CreateRefreshTokenAsync(existing.UserId, newRefreshHash, newRefreshExpiresAt);

        await repository.RevokeRefreshTokenAsync(existing.Id, now, newRefreshId);

        var user = await repository.GetByUserIdAsync(existing.UserId);
        if (user is null)
        {
            return Answer<RefreshResponse>.BadRequest("User not found");
        }

        var active = EnsureActive<RefreshResponse>(user);
        if (active is not null)
        {
            return active;
        }

        var (accessToken, accessExpiresAt) = jwtTokenGenerator.GenerateAccessToken(user);

        return Answer<RefreshResponse>.Ok(new RefreshResponse(
            accessToken,
            accessExpiresAt,
            newRefreshToken,
            newRefreshExpiresAt),
            "Token refreshed");
    }
}

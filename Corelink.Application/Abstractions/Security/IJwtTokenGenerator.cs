using Corelink.Application.Contracts.Auth;

namespace Corelink.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(UserAuthInfo user);
}

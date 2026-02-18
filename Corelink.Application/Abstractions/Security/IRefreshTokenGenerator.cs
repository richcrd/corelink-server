namespace Corelink.Application.Abstractions.Security;

public interface IRefreshTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateRefreshToken();
}

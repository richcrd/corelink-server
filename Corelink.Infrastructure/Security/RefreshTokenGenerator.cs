using System.Security.Cryptography;
using Corelink.Application.Abstractions.Security;
using Corelink.Application.Contracts.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Corelink.Infrastructure.Security;

public sealed class RefreshTokenGenerator(IOptions<JwtOptions> options) : IRefreshTokenGenerator
{
    private readonly JwtOptions _options = options.Value;

    public (string Token, DateTime ExpiresAt) GenerateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);

        // URL-safe string
        var token = Base64UrlEncoder.Encode(bytes.ToArray());
        var expires = DateTime.UtcNow.AddDays(_options.RefreshTokenDays);

        return (token, expires);
    }
}

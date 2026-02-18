using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Corelink.Application.Abstractions.Security;
using Corelink.Application.Contracts.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Corelink.Infrastructure.Security;

public sealed class JwtTokenGenerator(IOptions<JwtOptions> options) : IJwtTokenGenerator
{
    private readonly JwtOptions _options = options.Value;

    public (string Token, DateTime ExpiresAt) GenerateAccessToken(UserAuthInfo user)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_options.AccessTokenMinutes);

        var keyBytes = Encoding.UTF8.GetBytes(_options.SecretKey);
        var signingKey = new SymmetricSecurityKey(keyBytes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new("person_id", user.PersonId.ToString()),
            new(ClaimTypes.Role, user.RoleName)
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}

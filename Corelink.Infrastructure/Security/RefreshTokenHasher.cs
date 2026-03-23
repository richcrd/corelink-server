using System.Security.Cryptography;
using System.Text;
using Corelink.Application.Abstractions.Security;

namespace Corelink.Infrastructure.Security;

public sealed class RefreshTokenHasher : IRefreshTokenHasher
{
    public string Hash(string refreshToken)
    {
        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}

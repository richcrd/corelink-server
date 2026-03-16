namespace Corelink.Application.Abstractions.Security;

public interface IRefreshTokenHasher
{
    string Hash(string refreshToken);
}

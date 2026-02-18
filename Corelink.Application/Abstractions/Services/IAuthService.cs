using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Auth;

namespace Corelink.Application.Abstractions.Services;

public interface IAuthService
{
    Task<Answer<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Answer<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Answer<RefreshResponse>> RefreshAsync(RefreshRequest request);
}

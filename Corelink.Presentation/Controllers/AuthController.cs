using corelink_server.Common;
using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("service/[controller]")]
public sealed class AuthController(IAuthService service) : ResponseBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        return HandleResponse(await service.RegisterAsync(request));
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        return HandleResponse(await service.LoginAsync(request));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh([FromBody] RefreshRequest request)
    {
        return HandleResponse(await service.RefreshAsync(request));
    }
}

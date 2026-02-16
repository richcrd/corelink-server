using Corelink.Infrastructure.Persistence.Interface;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Corelink.Presentation.Controllers;

[ApiController]
[Route("api/db")]
public sealed class DbController : ControllerBase
{
    [HttpGet("ping")]
    public async Task<IActionResult> Ping([FromServices] IDbConnectionFactory factory, CancellationToken cancellationToken)
    {
        await using var connection = await factory.CreateOpenConnectionAsync(cancellationToken);
        var result = await connection.ExecuteScalarAsync<int>(new CommandDefinition("SELECT 1", cancellationToken: cancellationToken));
        return Ok(new { ok = result == 1 });
    }
}

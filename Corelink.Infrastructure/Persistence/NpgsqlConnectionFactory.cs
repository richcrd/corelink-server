using System.Data.Common;
using Corelink.Infrastructure.Persistence.Interface;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Corelink.Infrastructure.Persistence;

public sealed class NpgsqlConnectionFactory(IOptions<PostgresOptions> options) : IDbConnectionFactory
{
    private readonly PostgresOptions _options = options.Value;

    public async Task<DbConnection> CreateOpenConnectionAsync()
    {
        var connection = new NpgsqlConnection(_options.ConnectionString);
        await connection.OpenAsync();
        return connection;
    }
}

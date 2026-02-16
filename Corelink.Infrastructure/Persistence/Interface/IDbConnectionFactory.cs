using System.Data.Common;

namespace Corelink.Infrastructure.Persistence.Interface;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}

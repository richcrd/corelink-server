using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class DepartmentRepository(IDbConnectionFactory connectionFactory) : IDepartmentRepository
{
    public async Task<Guid> CreateAsync(Department department)
    {
        throw new NotImplementedException();
    }

    public async Task<Department?> GetByIdAsync(Guid id)
    {
        const string sql = """
                           SELECT 
                               id, 
                               name as Name, 
                               status::text AS Status 
                           FROM catalog_department 
                           WHERE id = @Id 
                           LIMIT 1;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Department>(sql, new { Id = id });
    }
}
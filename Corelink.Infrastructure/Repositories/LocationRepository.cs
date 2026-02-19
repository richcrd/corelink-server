using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class LocationRepository(IDbConnectionFactory connectionFactory) : ILocationRepository
{
    public async Task<Guid> CreateAsync(Location location)
    {
        const string sql = """
            INSERT INTO catalog_location
                (name, department_id, status)
            VALUES
                (@Name, @DepartmentId, @Status::status_enum)
            RETURNING id;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<Guid>(sql, new
        {
            location.Name,
            location.DepartmentId,
            Status = location.Status.ToDb()
        });
    }

    public async Task<Location?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id,
                name AS Name,
                department_id AS DepartmentId,
                status::text AS Status
            FROM catalog_location
            WHERE id = @Id
            LIMIT 1;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Location>(sql, new { Id = id });
    }

    public async Task<IReadOnlyList<Location>> ListByDepartmentIdAsync(Guid departmentId)
    {
        const string sql = """
            SELECT
                cl.id,
                cl.name AS Name,
                cd.name AS DepartmentName,
                cl.status::text AS Status
            FROM catalog_location cl
            LEFT JOIN catalog_department cd on cd.id = cl.department_id
            WHERE department_id = @DepartmentId
            ORDER BY name;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.QueryAsync<Location>(sql, new { DepartmentId = departmentId });
        return rows.AsList();
    }

    public async Task<bool> ExistsByNameInDepartmentAsync(string name, Guid departmentId)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1
                FROM catalog_location
                WHERE LOWER(name) = LOWER(@Name)
                  AND department_id = @DepartmentId
            );
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(sql, new { Name = name, DepartmentId = departmentId });
    }
}

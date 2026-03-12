using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class BranchRepository(IDbConnectionFactory connectionFactory) : IBranchRepository
{
    public async Task<long> CreateAsync(Branch location)
    {
        const string sql = """
            INSERT INTO catalog_branch
                (name, address, department_id, status)
            VALUES
                (@Name, @Address, @DepartmentId, @Status::status_enum)
            RETURNING id;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<long>(sql, new
        {
            location.Name,
            location.DepartmentId,
            Status = location.Status.ToDb()
        });
    }

    public async Task<Branch?> GetByIdAsync(long id)
    {
        const string sql = """
            SELECT
                id,
                name AS Name,
                department_id AS DepartmentId,
                status::text AS Status
            FROM catalog_branch
            WHERE id = @Id
            LIMIT 1;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Branch>(sql, new { Id = id });
    }

    public async Task<IReadOnlyList<Branch>> ListByDepartmentIdAsync(long departmentId)
    {
        const string sql = """
            SELECT
                cl.id,
                cl.name AS Name,
                cd.name AS DepartmentName,
                cl.status::text AS Status
            FROM catalog_branch cl
            LEFT JOIN catalog_department cd on cd.id = cl.department_id
            WHERE department_id = @DepartmentId
            ORDER BY name;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.QueryAsync<Branch>(sql, new { DepartmentId = departmentId });
        return rows.AsList();
    }

    public async Task<bool> ExistsByNameInDepartmentAsync(string name, long departmentId)
    {
        const string sql = """
            SELECT EXISTS(
                SELECT 1
                FROM catalog_branch
                WHERE LOWER(name) = LOWER(@Name)
                  AND department_id = @DepartmentId
            );
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<bool>(sql, new { Name = name, DepartmentId = departmentId });
    }
}

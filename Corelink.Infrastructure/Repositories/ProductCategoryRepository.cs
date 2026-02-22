using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class ProductCategoryRepository(IDbConnectionFactory connectionFactory) : IProductCategoryRepository
{
    public async Task<Guid> CreateAsync(ProductCategory productCategory)
    {
        const string sql = """
                            INSERT INTO product_category
                                 (name, description, status)
                            VALUES
                                 (@Name, @Description, @Status::status_enum)
                            RETURNING id;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<Guid>(sql, new
        {
            productCategory.Id,
            productCategory.Name,
            productCategory.Description,
            Status = productCategory.Status.ToDb()
        });
    }

    public async Task<ProductCategory?> GetById(Guid id)
    {
        const string sql = """
                           SELECT id, 
                           name AS Name, 
                           description as Description,
                           status::text as Status
                           FROM product_category
                           WHERE id = @Id
                           LIMIT 1;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<ProductCategory>(sql, new { Id = id });
    }

    public async Task<IReadOnlyList<ProductCategory>> GetAllAsync()
    {
        const string sql = """
                           SELECT
                               pc.id,
                               pc.name AS Name,
                               pc.description AS Description,
                               pc.status as Status
                           FROM product_category pc
                           ORDER BY pc.name;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var result = await connection.QueryAsync<ProductCategory>(sql);
        return result.ToList();
    }
}
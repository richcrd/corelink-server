using Corelink.Application.Contracts.ProductCategory;
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

    public async Task<IReadOnlyList<ProductCategoryListResponse>> GetAllAsync()
    {
        const string sql = """
                               SELECT 
                                   pc.id,
                                   pc.name,
                                   pc.description,
                                   img.url as ImageUrl
                               FROM product_category pc
                               LEFT JOIN product_category_image pci 
                                   ON pci.category_id = pc.id 
                                   AND pci.is_main = true
                               LEFT JOIN image img 
                                   ON img.id = pci.image_id
                               ORDER BY pc.name;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var result = await connection.QueryAsync<ProductCategoryListResponse>(sql);
        return result.ToList();
    }

    public async Task AddImageAsync(Guid categoryId, string imageUrl)
    {
        const string insertImageSql = """
                                          INSERT INTO image (url)
                                          VALUES (@Url)
                                          RETURNING id;
                                      """;

        const string relationSql = """
                                       INSERT INTO product_category_image
                                           (category_id, image_id, is_main)
                                       VALUES (@CategoryId, @ImageId, true);
                                   """;

        await using var connection =
            await connectionFactory.CreateOpenConnectionAsync();

        using var transaction = connection.BeginTransaction();

        try
        {
            var imageId = await connection.ExecuteScalarAsync<Guid>(
                insertImageSql,
                new { Url = imageUrl },
                transaction);

            await connection.ExecuteAsync(
                relationSql,
                new { CategoryId = categoryId, ImageId = imageId },
                transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
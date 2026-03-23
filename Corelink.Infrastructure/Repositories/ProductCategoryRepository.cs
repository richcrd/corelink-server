using Corelink.Application.Contracts.ProductCategory;
using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class ProductCategoryRepository(IDbConnectionFactory connectionFactory) : IProductCategoryRepository
{
    public async Task<long> CreateAsync(ProductCategory productCategory)
    {
        const string sql = """
                            INSERT INTO product_category
                                 (name, description, status)
                            VALUES
                                 (@Name, @Description, @Status::status_enum)
                            RETURNING id;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<long>(sql, new
        {
            productCategory.Id,
            productCategory.Name,
            productCategory.Description,
            Status = productCategory.Status.ToDb()
        });
    }

    public async Task<ProductCategory?> GetById(long id)
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
                               LEFT JOIN image img 
                                   ON img.id = pci.image_id
                               ORDER BY pc.name;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var result = await connection.QueryAsync<ProductCategoryListResponse>(sql);
        return result.ToList();
    }

    public async Task<bool> UpdateAsync(ProductCategory productCategory)
    {
        const string sql = """
                           UPDATE product_category
                           SET name = @Name,
                               description = @Description,
                               status = @Status::status_enum
                           WHERE id = @Id;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.ExecuteAsync(sql, new
        {
            productCategory.Id,
            productCategory.Name,
            productCategory.Description,
            productCategory.Status
        });
        return rows > 0;
    }

    public async Task UpdateImageAsync(long categoryId, long imageId)
    {
        const string deleteRelationsSql = """
                                      DELETE FROM product_category_image
                                      WHERE category_id = @CategoryId;
                                      """;

        const string insertRelationSql = """
                                      INSERT INTO product_category_image
                                          (category_id, image_id)
                                      VALUES (@CategoryId, @ImageId);
                                      """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        await using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(deleteRelationsSql, new { CategoryId = categoryId }, transaction);
            await connection.ExecuteAsync(insertRelationSql, new { CategoryId = categoryId, ImageId = imageId }, transaction);
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<string?> GetMainImageUrlAsync(long categoryId)
    {
        const string sql = """
                           SELECT img.url
                           FROM product_category_image pci
                           INNER JOIN image img ON img.id = pci.image_id
                           WHERE pci.category_id = @CategoryId
                           LIMIT 1;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<string>(sql, new { CategoryId = categoryId });
    }

    public async Task<long> CreateImageAsync(string url)
    {
        const string sql = """
                           INSERT INTO image (url)
                           VALUES (@Url)
                           RETURNING id;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<long>(sql, new { Url = url });
    }

    public async Task AddImageAsync(long categoryId, string imageUrl)
    {
        const string insertImageSql = """
                                          INSERT INTO image (url)
                                          VALUES (@Url)
                                          RETURNING id;
                                      """;

        const string relationSql = """
                                       INSERT INTO product_category_image
                                           (category_id, image_id)
                                       VALUES (@CategoryId, @ImageId);
                                   """;

        await using var connection =
            await connectionFactory.CreateOpenConnectionAsync();

        await using var transaction = connection.BeginTransaction();

        try
        {
            var imageId = await connection.ExecuteScalarAsync<long>(
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
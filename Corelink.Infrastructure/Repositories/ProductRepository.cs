using Corelink.Application.Contracts.Products;
using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class ProductRepository(IDbConnectionFactory connectionFactory) : IProductRepository
{
    public async Task<long> CreateAsync(Product product)
    {
        const string sql = """
                           INSERT INTO product
                           (name, description, category_id, status)
                           VALUES
                           (@Name, @Description, @CategoryId, @Status::status_enum)
                           RETURNING id;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<long>(sql, new
        {
            product.Name,
            product.Description,
            product.CategoryId,
            Status = product.Status.ToDb()
        });
    }

    public async Task<Product?> GetByIdAsync(long id)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();

        const string productSql = """
                                  SELECT id, name, description, category_id as CategoryId,
                                        status::text as Status
                                  FROM product
                                  WHERE id = @Id
                                  LIMIT 1;
                                  """;
        var product = await connection.QueryFirstOrDefaultAsync<Product>(productSql, new { Id = id });
        if (product is null)
            return null;

        const string imagesSql = """
                                 SELECT
                                     img.id,
                                     img.url
                                 FROM product_image pi
                                 INNER JOIN image img on img.id = pi.image_id
                                 WHERE pi.product_id = @ProductId;
                                 """;
        var images = await connection.QueryAsync<ProductImage>(imagesSql, new { ProductId = id });
        
        foreach (var img in images)
            product.AddImage(img);

        const string branchesSql = """
                                   SELECT id,
                                        branch_id as BranchId,
                                        price,
                                        status::text as Status
                                   FROM branch_product
                                   WHERE product_id = @ProductId;
                                   """;
        var branches = await connection.QueryAsync<ProductBranch>(branchesSql, new { ProductId = id });

        foreach (var branch in branches)
        {
            const string offersSql = """
                                     SELECT id,
                                            offer_price AS OfferPrice,
                                            start_date AS StartDate,
                                            end_date AS EndDate,
                                            status::text AS Status
                                        FROM product_offer
                                        WHERE branch_product_id = @BranchId;
                                     """;
            var offers = await connection.QueryAsync<ProductOffer>(offersSql, new { BranchId = branch.Id });

            foreach (var offer in offers)
                branch.AddOffer(offer);
            
            product.AddBranch(branch);
        }

        return product;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        const string sql = """
                           UPDATE product
                           SET name = @Name,
                               description = @Description,
                               status = @Status::status_enum
                           WHERE id = @Id;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.ExecuteAsync(sql, new
        {
            product.Id,
            product.Name,
            product.Description,
            Status = product.Status.ToDb()
        });
        return rows > 0;
    }

    public async Task<IReadOnlyList<ProductListResponse>> GetByBranchAsync(long branchId)
    {
        const string sql = """
                           SELECT
                                p.id as Id,
                                pb.id as BranchProductId,
                                p.name as Name,
                                pb.price as OriginalPrice,
                                po.offer_price as OfferPrice,
                                img.url as ImageUrl,
                                CASE
                                    WHEN po.id IS NOT NULL
                                        AND po.status = 'ACTIVE'
                                        AND (po.start_date is NULL OR po.start_date <= NOW())
                                        AND (po.end_date is NULL OR po.end_date >= NOW())
                                    THEN po.offer_price
                                    ELSE pb.price
                                END AS FinalPrice
                           FROM product p
                           INNER JOIN branch_product pb ON pb.product_id = p.id
                           LEFT JOIN product_offer po ON po.branch_product_id = pb.id
                           LEFT JOIN product_image pi ON pi.product_id = p.id
                           LEFT JOIN image img ON img.id = pi.image_id
                           WHERE pb.branch_id = @BranchId
                           AND p.status = 'ACTIVE'
                           AND pb.status = 'ACTIVE'
                           ORDER BY p.name;
                           """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();

        var result = await connection.QueryAsync<ProductListResponse>(sql, new { BranchId = branchId });
        return result.ToList();
    }

    public async Task<bool> AddToBranchAsync(long productId, long branchId, decimal price, int stock)
    {
        const string sql = """
                           INSERT INTO branch_product (product_id, branch_id, price, stock, status)
                           VALUES (@ProductId, @BranchId, @Price, @Stock, 'ACTIVE');
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.ExecuteAsync(sql, new
        {
            ProductId = productId,
            BranchId = branchId,
            Price = price,
            Stock = stock
        });

        return rows > 0;
    }

    public async Task<bool> AddOfferAsync(long productBranchId, decimal offerPrice, DateTime? startDate, DateTime? endDate)
    {
        const string sql = """
                           INSERT INTO product_offer
                           (branch_product_id, offer_price, start_date, end_date, status)
                           VALUES (@ProductBranchId, @OfferPrice, @StartDate, @EndDate, 'ACTIVE')
                           """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.ExecuteAsync(sql, new
        {
            ProductBranchId = productBranchId,
            OfferPrice = offerPrice,
            StartDate = startDate,
            EndDate = endDate
        });

        return rows > 0;
    }

    public async Task AddImageAsync(long productId, string imageUrl)
    {
        const string insertImageSql = """
                                      INSERT INTO image (url)
                                      VALUES (@Url)
                                      RETURNING id;
                                      """;
        const string relationSql = """
                                   INSERT INTO product_image
                                   (product_id, image_id)
                                   VALUES (@ProductId, @ImageId);
                                   """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        await using var transaction = connection.BeginTransaction();

        try
        {
            var imageId = await connection.ExecuteScalarAsync<long>(insertImageSql, new
            {
                Url = imageUrl,
            }, transaction);

            await connection.ExecuteAsync(relationSql, new
            {
                ProductId = productId,
                ImageId = imageId
            }, transaction);
            
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<string?> GetMainImageUrlAsync(long productId)
    {
        const string sql = """
                          SELECT img.url
                          FROM product_image pi
                          INNER JOIN image img ON img.id = pi.image_id
                          WHERE pi.product_id = @ProductId
                          LIMIT 1;
                          """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<string>(sql, new { ProductId = productId });
    }

    public async Task<(IReadOnlyList<ProductListResponse> Items, int TotalCount)> GetProductsByCategoryAndBranchAsync(long categoryId, long branchId, int page, int pageSize)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();

        const string countSql = """
                            SELECT COUNT(*)
                            FROM product p
                            INNER JOIN branch_product bp ON p.id = bp.product_id
                            WHERE p.category_id = @CategoryId
                              AND bp.branch_id = @BranchId
                              AND p.status = 'ACTIVE'
                              AND bp.status = 'ACTIVE'
                        """;

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { CategoryId = categoryId, BranchId = branchId });

        if (totalCount == 0)
        {
            return (Array.Empty<ProductListResponse>(), 0);
        }

        const string sql = """
                            SELECT
                                p.id as Id,
                                bp.id as BranchProductId,
                                p.name as Name,
                                img.url as ImageUrl,
                                bp.price as OriginalPrice,
                                po.offer_price as OfferPrice,
                                CASE
                                    WHEN po.id IS NOT NULL
                                        AND po.status = 'ACTIVE'
                                        AND (po.start_date is NULL OR po.start_date <= NOW())
                                        AND (po.end_date is NULL OR po.end_date >= NOW())
                                    THEN po.offer_price
                                    ELSE bp.price
                                END AS FinalPrice
                            FROM product p
                            INNER JOIN branch_product bp ON p.id = bp.product_id
                            LEFT JOIN product_offer po ON po.branch_product_id = bp.id
                            LEFT JOIN product_image pi ON pi.product_id = p.id
                            LEFT JOIN image img ON img.id = pi.image_id
                            WHERE p.category_id = @CategoryId
                              AND bp.branch_id = @BranchId
                              AND p.status = 'ACTIVE'
                              AND bp.status = 'ACTIVE'
                            ORDER BY p.name ASC
                            OFFSET @Offset LIMIT @Limit;
                        """;
                        
        var offset = (page - 1) * pageSize;
        var products = await connection.QueryAsync<ProductListResponse>(sql, new { 
            CategoryId = categoryId, 
            BranchId = branchId, 
            Offset = offset, 
            Limit = pageSize 
        });
        
        return (products.AsList(), totalCount);
    }
}
using Corelink.Application.Contracts.Products;
using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly IDbConnectionFactory connectionFactory;
    public ProductRepository(IDbConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<bool> AddToBranchAsync(long productId, long branchId, decimal price, int stock)
    {
        const string sql = @"INSERT INTO branch_product (product_id, branch_id, price, stock, status)
                             VALUES (@ProductId, @BranchId, @Price, @Stock, 'ACTIVE');";
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

    public async Task<bool> UpdateBranchProductAsync(long productId, long branchId, decimal? price, int? stock)
    {
        if (price is null && stock is null) return true;

        const string sql = @"UPDATE branch_product 
                             SET price = COALESCE(@Price, price),
                                 stock = COALESCE(@Stock, stock)
                             WHERE product_id = @ProductId AND branch_id = @BranchId;";
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteAsync(sql, new { ProductId = productId, BranchId = branchId, Price = price, Stock = stock }) > 0;
    }
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

        const string sql = """
                           SELECT
                               p.id, p.name, p.description,
                               p.category_id AS CategoryId,
                               p.status::text AS Status,
                               pb.id, pb.branch_id AS BranchId,
                               pb.price, pb.status::text AS Status,
                               po.id, po.offer_price AS OfferPrice,
                               po.start_date AS StartDate,
                               po.end_date AS EndDate,
                               po.status::text AS Status
                           FROM product p
                           LEFT JOIN branch_product pb ON pb.product_id = p.id
                           LEFT JOIN product_offer po ON po.branch_product_id = pb.id
                           WHERE p.id = @Id;
                           """;

        var productDict = new Dictionary<long, Product>();
        var branchDict = new Dictionary<long, ProductBranch>();

        await connection.QueryAsync<Product, ProductBranch?, ProductOffer?, Product>(
            sql,
            (product, branch, offer) =>
            {
                if (!productDict.TryGetValue(product.Id, out var p))
                {
                    p = product;
                    productDict[p.Id] = p;
                }

                if (branch is not null && !branchDict.ContainsKey(branch.Id))
                {
                    branchDict[branch.Id] = branch;
                    p.AddBranch(branch);
                }

                if (offer is not null && branch is not null
                    && branchDict.TryGetValue(branch.Id, out var b))
                {
                    try { b.AddOffer(offer); } catch { /* already added */ }
                }

                return p;
            },
            new { Id = id },
            splitOn: "id,id,id");

        return productDict.Values.FirstOrDefault();
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
        const string sql = @"SELECT 
            p.id AS Id,
            bp.id AS BranchProductId,
            p.name AS Name,
            img.url AS ImageUrl,
            bp.price AS OriginalPrice,
            active_offer.offer_price AS OfferPrice,
            COALESCE(active_offer.offer_price, bp.price) AS FinalPrice
        FROM product p
        INNER JOIN branch_product bp ON p.id = bp.product_id
        LEFT JOIN image img ON img.id = p.image_id
        LEFT JOIN LATERAL (
            SELECT po.offer_price
            FROM product_offer po
            WHERE po.branch_product_id = bp.id
              AND po.status = 'ACTIVE'
              AND (po.start_date IS NULL OR po.start_date <= NOW())
              AND (po.end_date IS NULL OR po.end_date >= NOW())
            ORDER BY po.start_date DESC
            LIMIT 1
        ) active_offer ON TRUE
        WHERE bp.branch_id = @BranchId
        ORDER BY FinalPrice DESC;";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var result = await connection.QueryAsync<ProductListResponse>(sql, new { BranchId = branchId });
        return result.AsList();
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

    public async Task<bool> AddOrReplaceImageAsync(long productId, string imageUrl)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        await using var transaction = connection.BeginTransaction();

        var oldImageId = await connection.ExecuteScalarAsync<long?>("SELECT image_id FROM product WHERE id = @ProductId", new { ProductId = productId }, transaction);

        var imageId = await connection.ExecuteScalarAsync<long>("INSERT INTO image (url) VALUES (@Url) RETURNING id;", new { Url = imageUrl }, transaction);

        await connection.ExecuteAsync("UPDATE product SET image_id = @ImageId WHERE id = @ProductId", new { ImageId = imageId, ProductId = productId }, transaction);

        if (oldImageId.HasValue)
            await connection.ExecuteAsync("DELETE FROM image WHERE id = @ImageId", new { ImageId = oldImageId.Value }, transaction);

        await transaction.CommitAsync();
        return true;
    }

    public async Task<string?> GetMainImageUrlAsync(long productId)
    {
        const string sql = """
                  SELECT img.url
                  FROM product p
                  LEFT JOIN image img ON img.id = p.image_id
                  WHERE p.id = @ProductId
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
                                active_offer.offer_price as OfferPrice,
                                COALESCE(active_offer.offer_price, bp.price) AS FinalPrice
                            FROM product p
                            INNER JOIN branch_product bp ON p.id = bp.product_id
                            LEFT JOIN image img ON img.id = p.image_id
                            LEFT JOIN LATERAL (
                                SELECT po.offer_price
                                FROM product_offer po
                                WHERE po.branch_product_id = bp.id
                                  AND po.status = 'ACTIVE'
                                  AND (po.start_date IS NULL OR po.start_date <= NOW())
                                  AND (po.end_date IS NULL OR po.end_date >= NOW())
                                ORDER BY po.start_date DESC
                                LIMIT 1
                            ) active_offer ON TRUE
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

    public async Task<IReadOnlyList<TopProductResponse>> GetTopProductsByBranchAsync(long branchId, int limit = 5)
    {
        const string sql = """
            SELECT 
                p.id AS ProductId,
                p.name AS ProductName,
                img.url AS ImageUrl,
                SUM(od.quantity) AS TotalSold
            FROM order_detail od
            INNER JOIN branch_product bp ON od.branch_product_id = bp.id
            INNER JOIN product p ON bp.product_id = p.id
            LEFT JOIN image img ON img.id = p.image_id
            WHERE bp.branch_id = @BranchId
            GROUP BY p.id, p.name, img.url
            ORDER BY TotalSold DESC
            LIMIT @Limit
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var result = await connection.QueryAsync<TopProductResponse>(sql, new { BranchId = branchId, Limit = limit });
        return result.AsList();
    }

    public async Task<IReadOnlyList<ProductListResponse>> GetTopProductsWithPriceByBranchAsync(long branchId, int limit = 5)
    {
        const string sql = @"SELECT 
            p.id AS Id,
            bp.id AS BranchProductId,
            p.name AS Name,
            img.url AS ImageUrl,
            bp.price AS OriginalPrice,
            COALESCE(active_offer.offer_price, bp.price) AS FinalPrice,
            active_offer.offer_price AS OfferPrice,
            COALESCE(SUM(od.quantity), 0) AS TotalSold
        FROM branch_product bp
        INNER JOIN product p ON bp.product_id = p.id
        LEFT JOIN image img ON img.id = p.image_id
        LEFT JOIN order_detail od ON od.branch_product_id = bp.id
        LEFT JOIN LATERAL (
            SELECT po.offer_price
            FROM product_offer po
            WHERE po.branch_product_id = bp.id
              AND po.status = 'ACTIVE'
              AND (po.start_date IS NULL OR po.start_date <= NOW())
              AND (po.end_date IS NULL OR po.end_date >= NOW())
            ORDER BY po.start_date DESC
            LIMIT 1
        ) active_offer ON TRUE
        WHERE bp.branch_id = @BranchId
        GROUP BY p.id, bp.id, p.name, img.url, bp.price, active_offer.offer_price
        ORDER BY TotalSold DESC
        LIMIT @Limit
        ";
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var result = await connection.QueryAsync<ProductListResponse>(sql, new { BranchId = branchId, Limit = limit });
        return result.AsList();
    }
}
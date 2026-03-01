using Corelink.Application.Contracts.Products;
using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class ProductRepository(IDbConnectionFactory connectionFactory) : IProductRepository
{
    public async Task<Guid> CreateAsync(Product product)
    {
        const string sql = """
                           INSERT INTO product
                           (name, description, category_id, status)
                           VALUES
                           (@Name, @Description, @CategoryId, @Status::status_enum)
                           RETURNING id;
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<Guid>(sql, new
        {
            product.Name,
            product.Description,
            product.CategoryId,
            Status = product.Status.ToDb()
        });
    }

    public async Task<Product?> GetByIdAsync(Guid id)
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
                                     img.url,
                                     pi.is_main as IsMain,
                                     pi.position
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
                                   FROM product_branch
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
                                        WHERE product_branch_id = @BranchId;
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

    public async Task<IReadOnlyList<ProductListResponse>> GetByBranchAsync(Guid branchId)
    {
        const string sql = """
                           SELECT
                                p.id,
                                p.name,
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
                           INNER JOIN product_branch pb ON pb.product_id = p.id
                           LEFT JOIN product_offer po ON po.product_branch_id = pb.id
                           LEFT JOIN product_image pi ON pi.product_id = p.id AND pi.is_main = true
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

    public async Task<bool> AddToBranchAsync(Guid productId, Guid branchId, decimal price)
    {
        const string sql = """
                           INSERT INTO product_branch (product_id, branch_id, price, status)
                           VALUES (@ProductId, @BranchId, @Price, 'ACTIVE');
                           """;
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.ExecuteAsync(sql, new
        {
            ProductId = productId,
            BranchId = branchId,
            Price = price
        });

        return rows > 0;
    }

    public async Task<bool> AddOfferAsync(Guid productBranchId, decimal offerPrice, DateTime? startDate, DateTime? endDate)
    {
        const string sql = """
                           INSERT INTO product_offer
                           (product_branch_id, offer_price, start_date, end_date, status)
                           VALUES (@ProductBranchId, @OfferPrice, @StartDate, @EndDate, 'ACTIVE')
                           """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.ExecuteAsync(sql, new
        {
            ProductBranchId = productBranchId,
            offerPrice = offerPrice,
            StartDate = startDate,
            EndDate = endDate
        });

        return rows > 0;
    }
}
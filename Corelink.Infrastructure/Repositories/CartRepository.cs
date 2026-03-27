using Corelink.Application.Contracts.Cart;
using Corelink.Application.Interface.Persistence;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class CartRepository(IDbConnectionFactory connectionFactory) : ICartRepository
{
    private const string CartItemSelectSql = """
        SELECT
            cd.id,
            cd.cart_id AS CartId,
            cd.branch_product_id AS BranchProductId,
            p.id AS ProductId,
            p.name AS ProductName,
            img.url AS ImageUrl,
            COALESCE(
                (
                    SELECT po.offer_price
                    FROM product_offer po
                    WHERE po.branch_product_id = bp.id
                      AND po.status = 'ACTIVE'
                      AND (po.start_date IS NULL OR po.start_date <= NOW())
                      AND (po.end_date IS NULL OR po.end_date >= NOW())
                    ORDER BY po.id DESC
                    LIMIT 1
                ),
                bp.price
            ) AS Price,
            cd.quantity AS Quantity
        FROM cart_detail cd
        INNER JOIN branch_product bp ON bp.id = cd.branch_product_id
        INNER JOIN product p ON p.id = bp.product_id
        LEFT JOIN image img ON img.id = p.image_id
        """;
    public async Task<Cart?> GetCartByUserId(long userId)
    {
        const string sql = """
            SELECT
                id,
                user_id AS UserId,
                branch_id AS BranchId
            FROM cart
            WHERE user_id = @UserId
              AND status = 'PENDING'
            ORDER BY id DESC
            LIMIT 1;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<Cart>(sql, new { UserId = userId });
    }

    public async Task<Cart> GetOrCreateCart(long userId, long branchId)
    {
        var current = await GetCartByUserId(userId);
        if (current is not null)
            return current;

        const string sql = """
            INSERT INTO cart
                (user_id, branch_id, status)
            VALUES
                (@UserId, @BranchId, 'PENDING')
            RETURNING id;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var id = await connection.ExecuteScalarAsync<long>(sql, new
        {
            UserId = userId,
            BranchId = branchId
        });

        return new Cart
        {
            Id = id,
            UserId = userId,
            BranchId = branchId,
            Items = Array.Empty<CartItem>()
        };
    }

    public async Task<IReadOnlyList<CartItem>> GetCartItems(long cartId)
    {
        var sql = CartItemSelectSql + "\nWHERE cd.cart_id = @CartId ORDER BY cd.id ASC;";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.QueryAsync<CartItem>(sql, new { CartId = cartId });
        return rows.AsList();
    }

    public async Task<CartItem?> GetCartItem(long cartId, long branchProductId)
    {
        var sql = CartItemSelectSql + "\nWHERE cd.cart_id = @CartId AND cd.branch_product_id = @BranchProductId LIMIT 1;";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<CartItem>(sql, new
        {
            CartId = cartId,
            BranchProductId = branchProductId
        });
    }

    public async Task AddItem(long cartId, long branchProductId, int quantity)
    {
        const string sql = """
            INSERT INTO cart_detail (cart_id, branch_product_id, quantity)
            SELECT @CartId, @BranchProductId, @Quantity
            FROM branch_product bp
            INNER JOIN product p ON p.id = bp.product_id
            WHERE bp.id = @BranchProductId
              AND bp.status = 'ACTIVE'
              AND p.status = 'ACTIVE'
              AND bp.stock >= @Quantity;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.ExecuteAsync(sql, new
        {
            CartId = cartId,
            BranchProductId = branchProductId,
            Quantity = quantity
        });

        if (rows == 0)
            throw new InvalidOperationException("Insufficient stock or inactive product");
    }

    public async Task UpdateQuantity(long cartItemId, int quantity)
    {
        const string sql = """
            UPDATE cart_detail cd
            SET quantity = @Quantity
            FROM branch_product bp
            INNER JOIN product p ON p.id = bp.product_id
            WHERE cd.id = @CartItemId
              AND bp.id = cd.branch_product_id
              AND bp.status = 'ACTIVE'
              AND p.status = 'ACTIVE'
              AND bp.stock >= @Quantity;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var rows = await connection.ExecuteAsync(sql, new
        {
            CartItemId = cartItemId,
            Quantity = quantity
        });

        if (rows == 0)
            throw new InvalidOperationException("Insufficient stock or inactive product");
    }

    public async Task RemoveItem(long cartId, long branchProductId)
    {
        const string sql = """
            DELETE FROM cart_detail
            WHERE cart_id = @CartId
              AND branch_product_id = @BranchProductId;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        await connection.ExecuteAsync(sql, new
        {
            CartId = cartId,
            BranchProductId = branchProductId
        });
    }

    public async Task ClearCart(long cartId)
    {
        const string sql = """
            DELETE FROM cart_detail
            WHERE cart_id = @CartId;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        await connection.ExecuteAsync(sql, new { CartId = cartId });
    }

    public async Task<BranchProduct?> GetBranchProduct(long branchProductId)
    {
        const string sql = """
            SELECT
                id,
                branch_id AS BranchId
            FROM branch_product
            WHERE id = @BranchProductId
              AND status = 'ACTIVE'
            LIMIT 1;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QueryFirstOrDefaultAsync<BranchProduct>(sql, new { BranchProductId = branchProductId });
    }
}

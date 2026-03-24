using Corelink.Application.Contracts.Checkout;
using Corelink.Application.Interface.Persistence;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class CheckoutRepository(IDbConnectionFactory connectionFactory) : ICheckoutRepository
{
    public async Task<string> GetCartStatusAsync(long cartId, long userId)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<string>(
            "SELECT status FROM cart WHERE id = @CartId AND user_id = @UserId", 
            new { CartId = cartId, UserId = userId });
    }

    public async Task<IEnumerable<CartCheckoutItem>> GetCartItemsForCheckoutAsync(long cartId)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        const string sql = """
            SELECT 
                cd.branch_product_id AS BranchProductId,
                p.name AS ProductName,
                bp.stock AS Stock,
                bp.status AS BranchProductStatus,
                p.status AS ProductStatus,
                cd.quantity AS Quantity,
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
                ) AS Price
            FROM cart_detail cd
            INNER JOIN branch_product bp ON bp.id = cd.branch_product_id
            INNER JOIN product p ON p.id = bp.product_id
            WHERE cd.cart_id = @CartId;
            """;

        return await connection.QueryAsync<CartCheckoutItem>(sql, new { CartId = cartId });
    }

    public async Task<long> ProcessCheckoutTransactionAsync(long cartId, long userId, decimal total, IEnumerable<CartCheckoutItem> items, long paymentMethodId, string? paymentReference)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            var branchId = await connection.ExecuteScalarAsync<long>(
                "SELECT branch_id FROM cart WHERE id = @CartId", new { CartId = cartId }, transaction);

            const string insertOrderSql = """
                INSERT INTO orders (user_id, branch_id, total, status)
                VALUES (@UserId, @BranchId, @Total, 'APPROVED')
                RETURNING id;
                """;

            var orderId = await connection.ExecuteScalarAsync<long>(insertOrderSql, new 
            {
                UserId = userId,
                BranchId = branchId,
                Total = total
            }, transaction);

            foreach (var item in items)
            {
                var subtotal = item.Price * item.Quantity;

                await connection.ExecuteAsync("""
                    INSERT INTO order_detail (order_id, branch_product_id, quantity, unit_price, subtotal)
                    VALUES (@OrderId, @BranchProductId, @Quantity, @UnitPrice, @Subtotal);
                    """, new 
                {
                    OrderId = orderId,
                    BranchProductId = item.BranchProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    Subtotal = subtotal
                }, transaction);

                await connection.ExecuteAsync("""
                    UPDATE branch_product SET stock = stock - @Quantity WHERE id = @BranchProductId;
                    """, new { Quantity = item.Quantity, BranchProductId = item.BranchProductId }, transaction);

                await connection.ExecuteAsync("""
                    INSERT INTO inventory_movement (branch_product_id, type, quantity, stock_before, stock_after, reference_type, reference_id, created_by)
                    VALUES (@BranchProductId, 'OUTBOUND', @Quantity, @StockBefore, @StockAfter, 'ORDER', @OrderId, @UserId);
                    """, new 
                {
                    BranchProductId = item.BranchProductId,
                    Quantity = item.Quantity,
                    StockBefore = item.Stock,
                    StockAfter = item.Stock - item.Quantity,
                    OrderId = orderId,
                    UserId = userId
                }, transaction);
            }

            await connection.ExecuteAsync("""
                INSERT INTO payment (order_id, payment_method_id, amount, reference, status)
                VALUES (@OrderId, @PaymentMethodId, @Amount, @Reference, 'APPROVED');
                """, new 
            {
                OrderId = orderId,
                PaymentMethodId = paymentMethodId,
                Amount = total,
                Reference = paymentReference ?? ""
            }, transaction);

            await connection.ExecuteAsync("UPDATE cart SET status = 'APPROVED', updated_at = NOW() WHERE id = @CartId;", 
                new { CartId = cartId }, transaction);

            transaction.Commit();
            return orderId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}

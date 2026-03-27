using Corelink.Application.Contracts.Orders;
using Corelink.Application.Interface.Persistence;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class OrderRepository(IDbConnectionFactory connectionFactory) : IOrderRepository
{
    private sealed record OrderHeaderDto(long OrderId, decimal Total, string Status, DateTime CreatedAt);

    public async Task<IReadOnlyList<OrderSummaryResponse>> GetOrdersAsync(long? userId = null)
    {
        var sql = """
            SELECT 
                o.id AS OrderId, 
                o.total AS Total, 
                o.status AS Status, 
                o.created_at AS CreatedAt,
                (p.first_name || ' ' || p.last_name) AS CustomerName,
                p.phone_number AS Phone,
                p.address AS Address,
                b.name AS BranchName,
                pm.name AS PaymentMethod
            FROM orders o
            INNER JOIN app_user au ON o.user_id = au.id
            INNER JOIN person p ON au.person_id = p.id
            INNER JOIN catalog_branch b ON o.branch_id = b.id
            LEFT JOIN LATERAL (
                SELECT payment_method_id 
                FROM payment 
                WHERE order_id = o.id 
                ORDER BY created_at DESC 
                LIMIT 1
            ) pay ON TRUE
            LEFT JOIN catalog_payment_method pm ON pay.payment_method_id = pm.id
            """;

        if (userId.HasValue)
        {
            sql += "\nWHERE o.user_id = @UserId";
        }

        sql += "\nORDER BY o.created_at DESC";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        var orders = await connection.QueryAsync<OrderSummaryResponse>(sql, new { UserId = userId });
        
        return orders.AsList();
    }

    public async Task<OrderDetailResponse?> GetOrderDetailsAsync(long orderId, long userId)
    {
        const string orderSql = """
            SELECT 
                id AS OrderId, 
                total AS Total, 
                status AS Status, 
                created_at AS CreatedAt
            FROM orders
            WHERE id = @OrderId AND user_id = @UserId
            """;

        const string itemsSql = """
            SELECT 
                p.name AS ProductName,
                img.url AS ImageUrl,
                od.quantity AS Quantity,
                od.unit_price AS UnitPrice,
                od.subtotal AS Subtotal
            FROM order_detail od
            INNER JOIN branch_product bp ON bp.id = od.branch_product_id
            INNER JOIN product p ON p.id = bp.product_id
            LEFT JOIN image img ON img.id = p.image_id
            WHERE od.order_id = @OrderId
            ORDER BY od.id ASC
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        
        var header = await connection.QueryFirstOrDefaultAsync<OrderHeaderDto>(orderSql, new { OrderId = orderId, UserId = userId });
        
        if (header is null)
            return null;

        var items = await connection.QueryAsync<OrderItemResponse>(itemsSql, new { OrderId = orderId });

        return new OrderDetailResponse(
            header.OrderId,
            header.Total,
            header.Status,
            header.CreatedAt,
            items.AsList()
        );
    }
}

using Corelink.Application.Abstractions.Services;
using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Orders;
using Corelink.Application.Interface.Persistence;

namespace Corelink.Application.Services;

public sealed class OrderService(IOrderRepository repository) : IOrderService
{
    public async Task<Answer<OrderDetailResponse>> GetOrderDetailsAsync(long orderId, long userId)
    {
        var order = await repository.GetOrderDetailsAsync(orderId, userId);
        
        if (order is null)
        {
            return Answer<OrderDetailResponse>.BadRequest("Orden no encontrada o no pertenece al usuario.");
        }

        return Answer<OrderDetailResponse>.Ok(order);
    }

    public async Task<Answer<IReadOnlyList<OrderSummaryResponse>>> GetOrdersAsync(long? userId = null)
    {
        var orders = await repository.GetOrdersAsync(userId);
        return Answer<IReadOnlyList<OrderSummaryResponse>>.Ok(orders);
    }
}

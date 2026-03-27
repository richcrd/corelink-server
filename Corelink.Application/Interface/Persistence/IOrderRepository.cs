using Corelink.Application.Contracts.Orders;

namespace Corelink.Application.Interface.Persistence;

public interface IOrderRepository
{
    Task<OrderDetailResponse?> GetOrderDetailsAsync(long orderId, long userId);
    Task<IReadOnlyList<OrderSummaryResponse>> GetOrdersAsync(long? userId = null);
}

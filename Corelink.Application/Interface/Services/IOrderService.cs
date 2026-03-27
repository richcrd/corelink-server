using Corelink.Application.Contracts;
using Corelink.Application.Contracts.Orders;

namespace Corelink.Application.Abstractions.Services;

public interface IOrderService
{
    Task<Answer<OrderDetailResponse>> GetOrderDetailsAsync(long orderId, long userId);
    Task<Answer<IReadOnlyList<OrderSummaryResponse>>> GetOrdersAsync(long? userId = null);
}

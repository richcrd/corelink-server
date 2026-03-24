using Corelink.Application.Contracts.Checkout;
using Corelink.Application.Interface.Persistence;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class PaymentMethodRepository(IDbConnectionFactory connectionFactory) : IPaymentMethodRepository
{
    public async Task<IEnumerable<PaymentMethodResponse>> GetActivePaymentMethodsAsync()
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        const string sql = "SELECT id AS Id, name AS Name FROM catalog_payment_method WHERE status = 'ACTIVE' ORDER BY id ASC;";
        return await connection.QueryAsync<PaymentMethodResponse>(sql);
    }
}

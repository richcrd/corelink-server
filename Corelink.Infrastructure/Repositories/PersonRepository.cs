using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class PersonRepository(IDbConnectionFactory connectionFactory) : IPersonRepository
{
    public async Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                id,
                first_name AS FirstName,
                last_name AS LastName,
                email AS Email,
                phone_number AS PhoneNumber,
                address AS Address,
                location_id AS LocationId,
                status::text AS Status,
                created_at AS CreatedAt
            FROM person
            WHERE id = @Id
            LIMIT 1;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Person>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Person?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                id,
                first_name AS FirstName,
                last_name AS LastName,
                email AS Email,
                phone_number AS PhoneNumber,
                address AS Address,
                location_id AS LocationId,
                status::text AS Status,
                created_at AS CreatedAt
            FROM person
            WHERE email = @Email
            LIMIT 1;
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Person>(
            new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken));
    }

    public async Task<Guid> CreateAsync(Person person, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO person
                (first_name, last_name, email, phone_number, address, location_id, status)
            VALUES
                (@FirstName, @LastName, @Email, @PhoneNumber, @Address, @LocationId, @Status::status_enum)
            RETURNING id;
            """;

        var parameters = new
        {
            person.FirstName,
            person.LastName,
            person.Email,
            person.PhoneNumber,
            person.Address,
            person.LocationId,
            person.Status
        };

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<Guid>(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }
}

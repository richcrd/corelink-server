using Corelink.Application.Interface.Persistence;
using Corelink.Domain.Entities;
using Corelink.Domain.Enums;
using Corelink.Infrastructure.Persistence.Interface;
using Dapper;

namespace Corelink.Infrastructure.Repositories;

public sealed class PersonRepository(IDbConnectionFactory connectionFactory) : IPersonRepository
{
    public async Task<Person?> GetByIdAsync(Guid id)
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

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Person>(sql, new { Id = id });
    }

    public async Task<Person?> GetByEmailAsync(string email)
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

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.QuerySingleOrDefaultAsync<Person>(sql, new { Email = email });
    }

    public async Task<Guid> CreateAsync(Person person)
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
            Status = person.Status.ToDb()
        };

        await using var connection = await connectionFactory.CreateOpenConnectionAsync();
        return await connection.ExecuteScalarAsync<Guid>(sql, parameters);
    }
}

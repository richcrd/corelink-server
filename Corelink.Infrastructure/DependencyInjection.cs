using Corelink.Infrastructure.Persistence;
using Corelink.Application.Interface.Persistence;
using Corelink.Infrastructure.Persistence.Interface;
using Corelink.Infrastructure.Persistence.Dapper;
using Corelink.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using System.Threading;

namespace Corelink.Infrastructure;

public static class DependencyInjection
{
    private static int _dapperConfigured;

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (Interlocked.Exchange(ref _dapperConfigured, 1) == 0)
        {
            SqlMapper.AddTypeHandler(new StatusEnumTypeHandler());
        }

        services.AddOptions<PostgresOptions>()
            .Configure(o =>
            {
                var fromConnectionStrings = configuration.GetConnectionString("CorelinkDb");
                var fromPostgresSection = configuration[$"{PostgresOptions.SectionName}:ConnectionString"];
                o.ConnectionString = !string.IsNullOrWhiteSpace(fromConnectionStrings)
                    ? fromConnectionStrings!
                    : (fromPostgresSection ?? string.Empty);
            })
            .Validate(o => !string.IsNullOrWhiteSpace(o.ConnectionString), "A Postgres connection string is required. Set ConnectionStrings:CorelinkDb or Postgres:ConnectionString")
            .ValidateOnStart();

        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

        // Repositories
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        return services;
    }
}

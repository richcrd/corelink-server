using Corelink.Infrastructure.Persistence;
using Corelink.Application.Interface.Persistence;
using Corelink.Infrastructure.Persistence.Interface;
using Corelink.Infrastructure.Persistence.Dapper;
using Corelink.Infrastructure.Repositories;
using Corelink.Application.Abstractions.Security;
using Corelink.Application.Contracts.Auth;
using Corelink.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using System.Threading;
using Corelink.Application.Contracts.Storage;
using Corelink.Infrastructure.Storage;

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
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.SecretKey), "Jwt:SecretKey is required")
            .Validate(o => o.AccessTokenMinutes > 0, "Jwt:AccessTokenMinutes must be > 0")
            .Validate(o => o.RefreshTokenDays > 0, "Jwt:RefreshTokenDays must be > 0")
            .ValidateOnStart();

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<IRefreshTokenHasher, RefreshTokenHasher>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddHttpClient<IFileService, SupabaseService>();

        return services;
    }
}

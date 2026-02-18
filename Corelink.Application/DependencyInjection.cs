using Corelink.Application.Abstractions.Services;
using Corelink.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corelink.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}

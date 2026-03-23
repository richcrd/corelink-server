using Corelink.Application.Abstractions.Services;
using Corelink.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Corelink.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<UploadImageHandler>();
        services.AddScoped<IProductCategoryService, ProductCategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICartService, CartService>();
        return services;
    }
}

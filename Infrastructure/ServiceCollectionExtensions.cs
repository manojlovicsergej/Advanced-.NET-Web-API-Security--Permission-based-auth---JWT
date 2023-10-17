using System.Reflection;
using Application.Services;
using Application.Services.Identity;
using Infrastructure.Context;
using Infrastructure.Services;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(("WebApi"))))
            .AddTransient<ApplicationDbSeeder>();
        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        return services
            .AddTransient<ITokenService, TokenService>()
            .AddTransient<IUserService, UserService>()
            .AddHttpContextAccessor()
            .AddScoped<ICurrentUserService, CurrentUserService>();
    }

    public static IServiceCollection AddEmployeeService(this IServiceCollection services)
    {
        return services.AddTransient<IEmployeeService, EmployeeService>();
    }

    public static void AddInfractureDependencies(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }
}
using EmployeeApi.Application.Abstractions.Repositories;
using EmployeeApi.Application.Abstractions.Services;
using EmployeeApi.Infrastructure.KeyVault;
using EmployeeApi.Infrastructure.Persistence;
using EmployeeApi.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName)));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();

        var vaultUrl = configuration["KeyVault:VaultUrl"]
            ?? throw new InvalidOperationException("KeyVault:VaultUrl configuration is missing.");

        services.AddSingleton<IKeyVaultService>(_ => new KeyVaultService(vaultUrl));

        return services;
    }
}

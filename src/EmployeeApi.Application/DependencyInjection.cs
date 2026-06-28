using EmployeeApi.Application.Abstractions.Messaging;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Employees.EmployeeMappingProfile>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<Employees.EmployeeMappingProfile>());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddAutoMapper(cfg =>
            cfg.AddProfile<Employees.EmployeeMappingProfile>());

        return services;
    }
}

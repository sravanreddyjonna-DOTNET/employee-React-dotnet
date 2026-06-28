using EmployeeApi.Domain.Entities;

namespace EmployeeApi.Application.Abstractions.Repositories;

/// <summary>
/// Defined here (Application) rather than in Infrastructure, so the business
/// logic depends only on an abstraction, never on EF Core directly
/// (Dependency Inversion Principle).
/// </summary>
public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken);

    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken);

    Task DeleteAsync(Employee employee, CancellationToken cancellationToken);
}

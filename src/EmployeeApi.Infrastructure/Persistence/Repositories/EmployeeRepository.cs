using EmployeeApi.Application.Abstractions.Repositories;
using EmployeeApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeApi.Infrastructure.Persistence.Repositories;

/// <summary>
/// The only class in the solution that knows EF Core exists. Implements the
/// abstraction defined in the Application layer (Dependency Inversion) — swap
/// this for Dapper, a different database, or an in-memory store, and nothing
/// above this layer needs to change.
/// </summary>
public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _dbContext;

    public EmployeeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken) =>
        await _dbContext.Employees
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Employee> AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return employee;
    }

    public async Task DeleteAsync(Employee employee, CancellationToken cancellationToken)
    {
        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

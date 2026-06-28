using MediatR;

namespace EmployeeApi.Application.Employees.Queries.GetAllEmployees;

// MediatR has a single IRequest<T> for both reads and writes — "command" vs
// "query" is a folder/naming convention here, not a type-level distinction.
public sealed record GetAllEmployeesQuery : IRequest<List<EmployeeDto>>;

using MediatR;

namespace EmployeeApi.Application.Employees.Commands.DeleteEmployee;

// MediatR.Unit is the library's built-in "void" result type for commands
// that don't return data.
public sealed record DeleteEmployeeCommand(int Id) : IRequest<Unit>;

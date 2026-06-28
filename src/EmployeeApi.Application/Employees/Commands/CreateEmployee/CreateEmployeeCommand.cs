using MediatR;

namespace EmployeeApi.Application.Employees.Commands.CreateEmployee;

public sealed record CreateEmployeeCommand(
    string Name,
    decimal Salary,
    string Dept,
    string Email,
    string Phone) : IRequest<CreateEmployeeResult>;

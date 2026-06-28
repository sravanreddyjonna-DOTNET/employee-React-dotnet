namespace EmployeeApi.Application.Employees.Commands.CreateEmployee;

public sealed record CreateEmployeeResult(
    int Id,
    string Name,
    decimal Salary,
    string Dept,
    string Email,
    string Phone,
    string SecretMessage);

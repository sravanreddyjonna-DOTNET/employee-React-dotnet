namespace EmployeeApi.Application.Employees;

public sealed record EmployeeDto(int Id, string Name, decimal Salary, string Dept, string Email, string Phone);

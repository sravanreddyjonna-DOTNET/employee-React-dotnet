namespace EmployeeApi.Api.Contracts;

public sealed record CreateEmployeeRequest(
    string Name,
    decimal Salary,
    string Dept,
    string Email,
    string Phone);

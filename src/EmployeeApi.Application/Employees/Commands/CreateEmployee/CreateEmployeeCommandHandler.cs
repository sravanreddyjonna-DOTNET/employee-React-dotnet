using EmployeeApi.Application.Abstractions.Repositories;
using EmployeeApi.Application.Abstractions.Services;
using EmployeeApi.Domain.Entities;
using MediatR;

namespace EmployeeApi.Application.Employees.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, CreateEmployeeResult>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IKeyVaultService _keyVaultService;

    public CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, IKeyVaultService keyVaultService)
    {
        _employeeRepository = employeeRepository;
        _keyVaultService = keyVaultService;
    }

    public async Task<CreateEmployeeResult> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = new Employee(request.Name, request.Salary, request.Dept, request.Email, request.Phone);
        var created = await _employeeRepository.AddAsync(employee, cancellationToken);

      //  var secretMessage = await _keyVaultService.GetSecretAsync("my-frist-Key", cancellationToken);

        return new CreateEmployeeResult(
            created.Id,
            created.Name,
            created.Salary,
            created.Dept,
            created.Email,
            created.Phone,
            "");
    }
}

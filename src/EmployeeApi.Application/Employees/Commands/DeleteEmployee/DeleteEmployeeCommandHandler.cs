using EmployeeApi.Application.Abstractions.Repositories;
using EmployeeApi.Application.Common.Exceptions;
using EmployeeApi.Domain.Entities;
using MediatR;

namespace EmployeeApi.Application.Employees.Commands.DeleteEmployee;

public sealed class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _employeeRepository;

    public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.Id);

        await _employeeRepository.DeleteAsync(employee, cancellationToken);

        return Unit.Value;
    }
}

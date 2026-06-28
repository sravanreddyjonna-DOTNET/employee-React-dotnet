using AutoMapper;
using EmployeeApi.Application.Abstractions.Repositories;
using MediatR;

namespace EmployeeApi.Application.Employees.Queries.GetAllEmployees;

public sealed class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, List<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public GetAllEmployeesQueryHandler(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<List<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<EmployeeDto>>(employees);
    }
}

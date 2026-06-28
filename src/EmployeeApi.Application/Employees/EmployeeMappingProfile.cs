using AutoMapper;
using EmployeeApi.Domain.Entities;

namespace EmployeeApi.Application.Employees;

public sealed class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<Employee, EmployeeDto>();
    }
}

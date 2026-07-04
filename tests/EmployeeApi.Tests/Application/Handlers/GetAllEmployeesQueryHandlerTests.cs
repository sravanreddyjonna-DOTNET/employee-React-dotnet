using AutoMapper;
using EmployeeApi.Application.Abstractions.Repositories;
using EmployeeApi.Application.Employees;
using EmployeeApi.Application.Employees.Queries.GetAllEmployees;
using EmployeeApi.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeApi.Tests.Application.Handlers;

public sealed class GetAllEmployeesQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly GetAllEmployeesQueryHandler _handler;

    public GetAllEmployeesQueryHandlerTests()
    {
        _handler = new GetAllEmployeesQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllEmployeesMapped()
    {
        var employees = new List<Employee>
        {
            new("John", 50000m, "IT",      "john@example.com", "+1234567890"),
            new("Jane", 60000m, "HR",      "jane@example.com", "+9876543210"),
            new("Bob",  70000m, "Finance", "bob@example.com",  "+1122334455")
        };
        var dtos = employees.Select(e => new EmployeeDto(0, e.Name, e.Salary, e.Dept, e.Email, e.Phone)).ToList();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(employees);
        _mapperMock.Setup(m => m.Map<List<EmployeeDto>>(employees)).Returns(dtos);

        var result = await _handler.Handle(new GetAllEmployeesQuery(), CancellationToken.None);

        result.Should().HaveCount(3);
        result[0].Name.Should().Be("John");
        result[1].Name.Should().Be("Jane");
        result[2].Name.Should().Be("Bob");
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyList()
    {
        var employees = new List<Employee>();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(employees);
        _mapperMock.Setup(m => m.Map<List<EmployeeDto>>(employees)).Returns(new List<EmployeeDto>());

        var result = await _handler.Handle(new GetAllEmployeesQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CallsRepositoryOnce()
    {
        var employees = new List<Employee>();
        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(employees);
        _mapperMock.Setup(m => m.Map<List<EmployeeDto>>(employees)).Returns(new List<EmployeeDto>());

        await _handler.Handle(new GetAllEmployeesQuery(), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

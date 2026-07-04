using EmployeeApi.Application.Abstractions.Repositories;
using EmployeeApi.Application.Abstractions.Services;
using EmployeeApi.Application.Employees.Commands.CreateEmployee;
using EmployeeApi.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeApi.Tests.Application.Handlers;

public sealed class CreateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly Mock<IKeyVaultService> _keyVaultMock = new();
    private readonly CreateEmployeeCommandHandler _handler;

    public CreateEmployeeCommandHandlerTests()
    {
        _handler = new CreateEmployeeCommandHandler(_repositoryMock.Object, _keyVaultMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsCorrectResult()
    {
        var command = new CreateEmployeeCommand("John", 50000m, "IT", "john@example.com", "+1234567890");
        var employee = new Employee(command.Name, command.Salary, command.Dept, command.Email, command.Phone);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("John");
        result.Salary.Should().Be(50000m);
        result.Dept.Should().Be("IT");
        result.Email.Should().Be("john@example.com");
        result.Phone.Should().Be("+1234567890");
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsRepositoryOnce()
    {
        var command = new CreateEmployeeCommand("Jane", 60000m, "HR", "jane@example.com", "+9876543210");
        var employee = new Employee(command.Name, command.Salary, command.Dept, command.Email, command.Phone);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsEmptySecretMessage()
    {
        var command = new CreateEmployeeCommand("John", 50000m, "IT", "john@example.com", "+1234567890");
        var employee = new Employee(command.Name, command.Salary, command.Dept, command.Email, command.Phone);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.SecretMessage.Should().BeEmpty();
    }
}

using EmployeeApi.Application.Abstractions.Repositories;
using EmployeeApi.Application.Common.Exceptions;
using EmployeeApi.Application.Employees.Commands.DeleteEmployee;
using EmployeeApi.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeApi.Tests.Application.Handlers;

public sealed class DeleteEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly DeleteEmployeeCommandHandler _handler;

    public DeleteEmployeeCommandHandlerTests()
    {
        _handler = new DeleteEmployeeCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingEmployee_DeletesAndReturnsUnit()
    {
        var employee = new Employee("John", 50000m, "IT", "john@example.com", "+1234567890");

        _repositoryMock
            .Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _repositoryMock
            .Setup(r => r.DeleteAsync(employee, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new DeleteEmployeeCommand(1), CancellationToken.None);

        result.Should().Be(MediatR.Unit.Value);
        _repositoryMock.Verify(r => r.DeleteAsync(employee, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingEmployee_ThrowsNotFoundException()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var act = async () => await _handler.Handle(new DeleteEmployeeCommand(99), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task Handle_NonExistingEmployee_NeverCallsDelete()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        var act = async () => await _handler.Handle(new DeleteEmployeeCommand(99), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

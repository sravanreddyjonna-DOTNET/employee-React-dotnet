using EmployeeApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace EmployeeApi.Tests.Domain;

public sealed class EmployeeTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var employee = new Employee("John", 50000m, "IT", "john@example.com", "+1234567890");

        employee.Name.Should().Be("John");
        employee.Salary.Should().Be(50000m);
        employee.Dept.Should().Be("IT");
        employee.Email.Should().Be("john@example.com");
        employee.Phone.Should().Be("+1234567890");
    }

    [Fact]
    public void Constructor_NewEmployee_HasDefaultId()
    {
        var employee = new Employee("Jane", 60000m, "HR", "jane@example.com", "+9876543210");

        employee.Id.Should().Be(0);
    }
}

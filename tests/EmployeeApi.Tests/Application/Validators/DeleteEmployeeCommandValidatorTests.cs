using EmployeeApi.Application.Employees.Commands.DeleteEmployee;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace EmployeeApi.Tests.Application.Validators;

public sealed class DeleteEmployeeCommandValidatorTests
{
    private readonly DeleteEmployeeCommandValidator _validator = new();

    [Fact]
    public void Id_GreaterThanZero_Passes()
    {
        var result = _validator.TestValidate(new DeleteEmployeeCommand(1));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Id_Zero_Fails()
    {
        var result = _validator.TestValidate(new DeleteEmployeeCommand(0));
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Id_Negative_Fails()
    {
        var result = _validator.TestValidate(new DeleteEmployeeCommand(-5));
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}

using EmployeeApi.Application.Employees.Commands.CreateEmployee;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace EmployeeApi.Tests.Application.Validators;

public sealed class CreateEmployeeCommandValidatorTests
{
    private readonly CreateEmployeeCommandValidator _validator = new();

    private static CreateEmployeeCommand ValidCommand() =>
        new("John Doe", 50000m, "IT", "john@example.com", "+1234567890");

    [Fact]
    public void Valid_Command_Passes()
    {
        var result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    // ── Name ──────────────────────────────────────────────────────────────

    [Fact]
    public void Name_Empty_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Name = "" });
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_TooLong_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Name = new string('A', 101) });
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Name_MaxLength_Passes()
    {
        var result = _validator.TestValidate(ValidCommand() with { Name = new string('A', 100) });
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    // ── Salary ────────────────────────────────────────────────────────────

    [Fact]
    public void Salary_Zero_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Salary = 0 });
        result.ShouldHaveValidationErrorFor(x => x.Salary);
    }

    [Fact]
    public void Salary_Negative_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Salary = -1 });
        result.ShouldHaveValidationErrorFor(x => x.Salary);
    }

    [Fact]
    public void Salary_Positive_Passes()
    {
        var result = _validator.TestValidate(ValidCommand() with { Salary = 1 });
        result.ShouldNotHaveValidationErrorFor(x => x.Salary);
    }

    // ── Department ────────────────────────────────────────────────────────

    [Fact]
    public void Dept_Empty_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Dept = "" });
        result.ShouldHaveValidationErrorFor(x => x.Dept);
    }

    [Fact]
    public void Dept_TooLong_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Dept = new string('A', 51) });
        result.ShouldHaveValidationErrorFor(x => x.Dept);
    }

    // ── Email ─────────────────────────────────────────────────────────────

    [Fact]
    public void Email_Empty_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Email = "" });
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    public void Email_InvalidFormat_Fails(string email)
    {
        var result = _validator.TestValidate(ValidCommand() with { Email = email });
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Email_TooLong_Fails()
    {
        var email = new string('a', 141) + "@example.com";
        var result = _validator.TestValidate(ValidCommand() with { Email = email });
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Email_Valid_Passes()
    {
        var result = _validator.TestValidate(ValidCommand() with { Email = "valid@domain.com" });
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    // ── Phone ─────────────────────────────────────────────────────────────

    [Fact]
    public void Phone_Empty_Fails()
    {
        var result = _validator.TestValidate(ValidCommand() with { Phone = "" });
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abc")]
    [InlineData("0123456")]
    public void Phone_InvalidFormat_Fails(string phone)
    {
        var result = _validator.TestValidate(ValidCommand() with { Phone = phone });
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Theory]
    [InlineData("+1234567890")]
    [InlineData("1234567890")]
    [InlineData("+919876543210")]
    public void Phone_ValidFormat_Passes(string phone)
    {
        var result = _validator.TestValidate(ValidCommand() with { Phone = phone });
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
    }
}

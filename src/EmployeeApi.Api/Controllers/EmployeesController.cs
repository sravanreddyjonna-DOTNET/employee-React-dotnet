using EmployeeApi.Api.Contracts;
using EmployeeApi.Application.Employees;
using EmployeeApi.Application.Employees.Commands.CreateEmployee;
using EmployeeApi.Application.Employees.Commands.DeleteEmployee;
using EmployeeApi.Application.Employees.Queries.GetAllEmployees;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Api.Controllers;

[ApiController]
[Route("api/employees")]
public sealed class EmployeesController : ControllerBase
{
    private readonly ISender _sender;

    // Depends only on ISender — never on a concrete handler. Adding a fourth
    // endpoint later means adding one action here, not touching this constructor.
    public EmployeesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Returns all employees.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EmployeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _sender.Send(new GetAllEmployeesQuery(), cancellationToken);
        return Ok(employees);
    }

    /// <summary>Creates a new employee.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateEmployeeCommand(request.Name, request.Salary, request.Dept, request.Email, request.Phone);
        var employee = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetAll), employee);
    }

    /// <summary>Deletes an employee by id.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteEmployeeCommand(id), cancellationToken);
        return NoContent();
    }
}

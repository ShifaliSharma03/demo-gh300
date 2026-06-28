using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeApi.Data;
using EmployeeApi.DTOs;
using EmployeeApi.Models;

namespace EmployeeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(AppDbContext context, ILogger<EmployeesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>Gets all employees.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAll()
    {
        _logger.LogInformation("Retrieving all employees");
        var employees = await _context.Employees.ToListAsync();
        return Ok(employees);
    }

    /// <summary>Gets an employee by ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Employee>> GetById(int id)
    {
        _logger.LogInformation("Retrieving employee with ID {EmployeeId}", id);
        var employee = await _context.Employees.FindAsync(id);

        if (employee is null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", id);
            return NotFound(new { message = $"Employee with ID {id} not found." });
        }

        return Ok(employee);
    }

    /// <summary>Creates a new employee.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Employee>> Create([FromBody] CreateEmployeeDto dto)
    {
        _logger.LogInformation("Creating employee with email {Email}", dto.Email);

        if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
        {
            _logger.LogWarning("Employee with email {Email} already exists", dto.Email);
            return Conflict(new { message = $"An employee with email '{dto.Email}' already exists." });
        }

        var employee = new Employee
        {
            FirstName  = dto.FirstName,
            LastName   = dto.LastName,
            Email      = dto.Email,
            Department = dto.Department,
            JobTitle   = dto.JobTitle,
            Salary     = dto.Salary,
            HireDate   = dto.HireDate,
            IsActive   = dto.IsActive
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Employee {EmployeeId} created successfully", employee.Id);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    /// <summary>Updates an existing employee.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Employee>> Update(int id, [FromBody] UpdateEmployeeDto dto)
    {
        _logger.LogInformation("Updating employee with ID {EmployeeId}", id);

        var employee = await _context.Employees.FindAsync(id);
        if (employee is null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found for update", id);
            return NotFound(new { message = $"Employee with ID {id} not found." });
        }

        if (await _context.Employees.AnyAsync(e => e.Email == dto.Email && e.Id != id))
        {
            _logger.LogWarning("Email {Email} is already in use by another employee", dto.Email);
            return Conflict(new { message = $"Email '{dto.Email}' is already in use by another employee." });
        }

        employee.FirstName  = dto.FirstName;
        employee.LastName   = dto.LastName;
        employee.Email      = dto.Email;
        employee.Department = dto.Department;
        employee.JobTitle   = dto.JobTitle;
        employee.Salary     = dto.Salary;
        employee.HireDate   = dto.HireDate;
        employee.IsActive   = dto.IsActive;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Employee {EmployeeId} updated successfully", id);
        return Ok(employee);
    }

    /// <summary>Deletes an employee by ID.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting employee with ID {EmployeeId}", id);

        var employee = await _context.Employees.FindAsync(id);
        if (employee is null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found for deletion", id);
            return NotFound(new { message = $"Employee with ID {id} not found." });
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Employee {EmployeeId} deleted successfully", id);
        return NoContent();
    }
}

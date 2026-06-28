using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using EmployeeApi.Controllers;
using EmployeeApi.Data;
using EmployeeApi.DTOs;
using EmployeeApi.Models;

namespace EmployeeApi.Tests;

public class EmployeesControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly EmployeesController _controller;

    public EmployeesControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _controller = new EmployeesController(_context, NullLogger<EmployeesController>.Instance);
    }

    public void Dispose() => _context.Dispose();

    private static CreateEmployeeDto CreateDto(string email = "alice@example.com") => new()
    {
        FirstName  = "Alice",
        LastName   = "Smith",
        Email      = email,
        Department = "Engineering",
        JobTitle   = "Developer",
        Salary     = 80_000m,
        HireDate   = new DateTime(2022, 1, 15, 0, 0, 0, DateTimeKind.Utc),
        IsActive   = true
    };

    // ─── GetAll ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_ReturnsEmptyList_WhenNoEmployees()
    {
        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var employees = Assert.IsAssignableFrom<IEnumerable<Employee>>(ok.Value);
        Assert.Empty(employees);
    }

    [Fact]
    public async Task GetAll_ReturnsAllEmployees()
    {
        await _controller.Create(CreateDto("a@example.com"));
        await _controller.Create(CreateDto("b@example.com"));

        var result = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var employees = Assert.IsAssignableFrom<IEnumerable<Employee>>(ok.Value);
        Assert.Equal(2, employees.Count());
    }

    // ─── GetById ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetById_ReturnsEmployee_WhenFound()
    {
        var created = await _controller.Create(CreateDto());
        var createdResult = Assert.IsType<CreatedAtActionResult>(created.Result);
        var employee = Assert.IsType<Employee>(createdResult.Value);

        var result = await _controller.GetById(employee.Id);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var found = Assert.IsType<Employee>(ok.Value);
        Assert.Equal(employee.Id, found.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var result = await _controller.GetById(999);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    // ─── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_ReturnsCreated_WithValidData()
    {
        var result = await _controller.Create(CreateDto());
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var employee = Assert.IsType<Employee>(created.Value);
        Assert.Equal("Alice", employee.FirstName);
        Assert.Equal(201, created.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsConflict_WhenEmailExists()
    {
        await _controller.Create(CreateDto());
        var result = await _controller.Create(CreateDto());
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    // ─── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Update_ReturnsUpdatedEmployee()
    {
        var created = (CreatedAtActionResult)(await _controller.Create(CreateDto())).Result!;
        var employee = (Employee)created.Value!;

        var updateDto = new UpdateEmployeeDto
        {
            FirstName  = "Bob",
            LastName   = "Jones",
            Email      = "alice@example.com",
            Department = "HR",
            JobTitle   = "Manager",
            Salary     = 90_000m,
            HireDate   = employee.HireDate,
            IsActive   = false
        };

        var result = await _controller.Update(employee.Id, updateDto);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var updated = Assert.IsType<Employee>(ok.Value);
        Assert.Equal("Bob", updated.FirstName);
        Assert.Equal("HR", updated.Department);
        Assert.False(updated.IsActive);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMissing()
    {
        var dto = new UpdateEmployeeDto
        {
            FirstName  = "Bob",
            LastName   = "Jones",
            Email      = "bob@example.com",
            Department = "HR",
            JobTitle   = "Manager",
            Salary     = 90_000m,
            HireDate   = DateTime.UtcNow,
            IsActive   = true
        };

        var result = await _controller.Update(999, dto);
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsConflict_WhenEmailTakenByAnother()
    {
        await _controller.Create(CreateDto("alice@example.com"));
        var second = (CreatedAtActionResult)(await _controller.Create(CreateDto("bob@example.com"))).Result!;
        var secondEmployee = (Employee)second.Value!;

        var dto = new UpdateEmployeeDto
        {
            FirstName  = "Bob",
            LastName   = "Jones",
            Email      = "alice@example.com", // already owned by first employee
            Department = "HR",
            JobTitle   = "Manager",
            Salary     = 90_000m,
            HireDate   = DateTime.UtcNow,
            IsActive   = true
        };

        var result = await _controller.Update(secondEmployee.Id, dto);
        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    // ─── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenFound()
    {
        var created = (CreatedAtActionResult)(await _controller.Create(CreateDto())).Result!;
        var employee = (Employee)created.Value!;

        var result = await _controller.Delete(employee.Id);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        var result = await _controller.Delete(999);
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_RemovesEmployeeFromDatabase()
    {
        var created = (CreatedAtActionResult)(await _controller.Create(CreateDto())).Result!;
        var employee = (Employee)created.Value!;

        await _controller.Delete(employee.Id);

        var getResult = await _controller.GetById(employee.Id);
        Assert.IsType<NotFoundObjectResult>(getResult.Result);
    }
}

using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.Models;

public class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [MaxLength(100, ErrorMessage = "Department cannot exceed 100 characters.")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Job title is required.")]
    [MaxLength(150, ErrorMessage = "Job title cannot exceed 150 characters.")]
    public string JobTitle { get; set; } = string.Empty;

    [Range(0.01, 10_000_000, ErrorMessage = "Salary must be between 0.01 and 10,000,000.")]
    public decimal Salary { get; set; }

    [Required]
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}

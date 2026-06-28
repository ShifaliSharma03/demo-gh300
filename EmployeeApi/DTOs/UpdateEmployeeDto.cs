using System.ComponentModel.DataAnnotations;

namespace EmployeeApi.DTOs;

public class UpdateEmployeeDto
{
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Job title is required.")]
    [MaxLength(150)]
    public string JobTitle { get; set; } = string.Empty;

    [Range(0.01, 10_000_000)]
    public decimal Salary { get; set; }

    public DateTime HireDate { get; set; }

    public bool IsActive { get; set; }
}

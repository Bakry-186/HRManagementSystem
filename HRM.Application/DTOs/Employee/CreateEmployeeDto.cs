namespace HRM.Application.DTOs.Employee;

public class CreateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateOnly HireDate { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}

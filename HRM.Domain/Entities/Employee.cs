namespace HRM.Domain.Entities;

public class Employee : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required DateOnly HireDate { get; set; }
    public required string JobTitle { get; set; }
    public required decimal Salary { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation property
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
}

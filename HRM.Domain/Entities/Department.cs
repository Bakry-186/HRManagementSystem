namespace HRM.Domain.Entities;

public class Department : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation property
    public ICollection<Employee> Employees { get; set; } = [];
}

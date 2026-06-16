namespace HRM.Domain.Entities;

public class Department : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    // Navigation property
    public ICollection<Employee> Employees { get; set; } = [];
}

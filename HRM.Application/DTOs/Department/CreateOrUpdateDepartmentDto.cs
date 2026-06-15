namespace HRM.Application.DTOs.Department;

public class CreateOrUpdateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

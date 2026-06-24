namespace HRM.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required string Role { get; set; }
}

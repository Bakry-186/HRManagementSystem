namespace HRM.Application.DTOs.Auth;

public class RegisterResultDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

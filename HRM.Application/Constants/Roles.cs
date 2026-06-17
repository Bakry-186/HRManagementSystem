namespace HRM.Application.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string HR = "HR";
    public const string Viewer = "Viewer";

    public static readonly string[] All = [Admin, HR, Viewer];
}

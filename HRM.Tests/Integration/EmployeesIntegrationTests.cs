using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using HRM.Application.Constants;
using HRM.Application.DTOs.Employee;

namespace HRM.Tests.Integration;

public class EmployeesIntegrationTests : IClassFixture<HrmWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly HrmWebApplicationFactory _factory;

    public EmployeesIntegrationTests(HrmWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllEmployees_WithoutToken_Returns401Unauthorized()
    {
        var response = await _client.GetAsync("/api/v1/employees");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAllEmployees_WithValidToken_Returns200OK()
    {
        var token = _factory.GenerateJwtToken("testuser", Roles.Viewer);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/v1/employees");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateEmployee_WithInvalidBody_Returns400BadRequest()
    {
        var token = _factory.GenerateJwtToken("adminuser", Roles.Admin);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var dto = new CreateEmployeeDto
        {
            FirstName = "",
            LastName = "",
            Email = "not-an-email",
            HireDate = DateOnly.MinValue,
            JobTitle = "",
            Salary = -1
        };

        var response = await _client.PostAsJsonAsync("/api/v1/employees", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetEmployeeById_WithUnknownId_Returns404NotFound()
    {
        var token = _factory.GenerateJwtToken("testuser", Roles.Viewer);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync($"/api/v1/employees/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

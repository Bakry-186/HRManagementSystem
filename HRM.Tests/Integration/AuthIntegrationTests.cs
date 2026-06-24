using System.Net;
using System.Net.Http.Json;
using HRM.Application.Constants;
using HRM.Application.DTOs.Auth;

namespace HRM.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<HrmWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(HrmWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_Returns201Created()
    {
        var dto = new RegisterDto
        {
            Username = $"user_{Guid.NewGuid():N}",
            Password = "Password123!",
            Role = Roles.Viewer
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithToken()
    {
        var username = $"user_{Guid.NewGuid():N}";
        var password = "Password123!";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterDto
        {
            Username = username,
            Password = password,
            Role = Roles.Viewer
        });
        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginDto
        {
            Username = username,
            Password = password
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(body?.Token);
        Assert.NotEmpty(body.Token);
    }

    private record TokenResponse(string Token);
}

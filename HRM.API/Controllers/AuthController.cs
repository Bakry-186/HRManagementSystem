using Asp.Versioning;
using HRM.Application.Common.Models;
using HRM.Application.DTOs.Auth;
using HRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await authService.RegisterAsync(dto);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await authService.LoginAsync(dto);

        if (token is null)
        {
            return Unauthorized(new ApiErrorResponse
            {
                StatusCode = 401,
                Message = "Invalid username or password."
            });
        }
        return Ok(new { token });
    }
}

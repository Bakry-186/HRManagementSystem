using Asp.Versioning;
using HRM.Application.Common.Models;
using HRM.Application.DTOs.Employee;
using HRM.Application.Features.Employees.Queries.GetAllEmployeesV2;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/employees")]
[Produces("application/json")]
[Authorize]
public class EmployeesV2Controller(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeV2ResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllEmployeesV2Query(pageNumber, pageSize));
        return Ok(result);
    }
}

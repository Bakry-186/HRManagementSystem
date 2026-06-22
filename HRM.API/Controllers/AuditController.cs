using HRM.Application.Common.Models;
using HRM.Application.Constants;
using HRM.Application.DTOs.AuditLog;
using HRM.Application.Features.AuditLogs.Queries.GetAllAuditLogs;
using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Authorize(Roles = Roles.Admin)]
public class AuditController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AuditLogResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? entityName = null,
        [FromQuery] Guid? entityId = null)
    {
        var result = await mediator.Send(new GetAllAuditLogsQuery(pageNumber, pageSize, entityName, entityId));
        return Ok(result);
    }
}

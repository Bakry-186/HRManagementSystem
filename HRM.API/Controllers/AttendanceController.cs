using HRM.Application.Common.Models;
using HRM.Application.Constants;
using HRM.Application.DTOs.AttendanceRecord;
using HRM.Application.Features.AttendanceRecords.Commands.CreateAttendanceRecord;
using HRM.Application.Features.AttendanceRecords.Commands.DeactivateAttendanceRecord;
using HRM.Application.Features.AttendanceRecords.Commands.UpdateAttendanceRecord;
using HRM.Application.Features.AttendanceRecords.Queries.GetAllAttendanceRecords;
using HRM.Application.Features.AttendanceRecords.Queries.GetAttendanceRecordById;
using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Authorize]
public class AttendanceController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AttendanceRecordResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllAttendanceRecordsQuery(pageNumber, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AttendanceRecordResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetAttendanceRecordByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.HR}")]
    [ProducesResponseType(typeof(AttendanceRecordResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateAttendanceRecordDto dto)
    {
        var result = await mediator.Send(new CreateAttendanceRecordCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.HR}")]
    [ProducesResponseType(typeof(AttendanceRecordResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAttendanceRecordDto dto)
    {
        var result = await mediator.Send(new UpdateAttendanceRecordCommand(id, dto));
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await mediator.Send(new DeactivateAttendanceRecordCommand(id));
        return NoContent();
    }
}

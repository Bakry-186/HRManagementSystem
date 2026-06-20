using HRM.Application.Common.Models;
using HRM.Application.Constants;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Features.PayrollRecords.Commands.CalculatePayroll;
using HRM.Application.Features.PayrollRecords.Commands.CreatePayrollRecord;
using HRM.Application.Features.PayrollRecords.Commands.DeactivatePayrollRecord;
using HRM.Application.Features.PayrollRecords.Commands.UpdatePayrollRecord;
using HRM.Application.Features.PayrollRecords.Commands.UpdatePayrollStatus;
using HRM.Application.Features.PayrollRecords.Queries.GetAllPayrollRecords;
using HRM.Application.Features.PayrollRecords.Queries.GetPayrollRecordById;
using HRM.Application.Features.PayrollRecords.Queries.GetPayrollRecordsByEmployeeId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRM.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class PayrollController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PayrollRecordResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetAllPayrollRecordsQuery(pageNumber, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PayrollRecordResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetPayrollRecordByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("employee/{employeeId:guid}")]
    [ProducesResponseType(typeof(PagedResult<PayrollRecordResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmployeeId(Guid employeeId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var result = await mediator.Send(new GetPayrollRecordsByEmployeeIdQuery(employeeId, pageNumber, pageSize));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.HR}")]
    [ProducesResponseType(typeof(PayrollRecordResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreatePayrollRecordDto dto)
    {
        var result = await mediator.Send(new CreatePayrollRecordCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("calculate")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.HR}")]
    [ProducesResponseType(typeof(PayrollRecordResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Calculate([FromBody] CalculatePayrollDto dto)
    {
        var result = await mediator.Send(new CalculatePayrollCommand(dto));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    
    [HttpPut("{id:guid}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.HR}")]
    [ProducesResponseType(typeof(PayrollRecordResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePayrollRecordDto dto)
    {
        var result = await mediator.Send(new UpdatePayrollRecordCommand(id, dto));
        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.HR}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePayrollRecordStatusDto dto)
    {
        await mediator.Send(new UpdatePayrollStatusCommand(id, dto));
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await mediator.Send(new DeactivatePayrollRecordCommand(id));
        return NoContent();
    }
}

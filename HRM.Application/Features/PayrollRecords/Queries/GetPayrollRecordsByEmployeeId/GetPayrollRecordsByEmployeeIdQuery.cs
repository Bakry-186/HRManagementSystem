using HRM.Application.Common.Models;
using HRM.Application.DTOs.PayrollRecord;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Queries.GetPayrollRecordsByEmployeeId;

public record GetPayrollRecordsByEmployeeIdQuery(Guid EmployeeId, int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<PayrollRecordResponseDto>>;
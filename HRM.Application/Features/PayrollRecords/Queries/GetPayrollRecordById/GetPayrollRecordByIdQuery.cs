using HRM.Application.DTOs.PayrollRecord;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Queries.GetPayrollRecordById;

public record GetPayrollRecordByIdQuery(Guid Id) : IRequest<PayrollRecordResponseDto>;

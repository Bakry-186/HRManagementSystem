using HRM.Application.DTOs.PayrollRecord;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Commands.CalculatePayroll;

public record CalculatePayrollCommand(CalculatePayrollDto Dto) : IRequest<PayrollRecordResponseDto>;

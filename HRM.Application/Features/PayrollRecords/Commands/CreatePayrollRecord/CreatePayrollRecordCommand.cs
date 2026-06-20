using HRM.Application.DTOs.PayrollRecord;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Commands.CreatePayrollRecord;

public record CreatePayrollRecordCommand(CreatePayrollRecordDto Dto) : IRequest<PayrollRecordResponseDto>;

using HRM.Application.DTOs.PayrollRecord;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Commands.UpdatePayrollRecord;

public record UpdatePayrollRecordCommand(Guid Id, UpdatePayrollRecordDto Dto) : IRequest<PayrollRecordResponseDto>;

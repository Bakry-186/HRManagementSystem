using HRM.Application.DTOs.PayrollRecord;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Commands.UpdatePayrollStatus;

public record UpdatePayrollStatusCommand(Guid Id, UpdatePayrollRecordStatusDto Dto) : IRequest<bool>;
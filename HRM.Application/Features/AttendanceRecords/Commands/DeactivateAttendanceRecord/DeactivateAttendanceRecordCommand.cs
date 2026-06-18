using MediatR;

namespace HRM.Application.Features.AttendanceRecords.Commands.DeactivateAttendanceRecord;

public record DeactivateAttendanceRecordCommand(Guid Id) : IRequest<bool>;

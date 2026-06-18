using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.AttendanceRecords.Commands.DeactivateAttendanceRecord;

public class DeactivateAttendanceRecordCommandHandler(IAttendanceRepository repository)
    : IRequestHandler<DeactivateAttendanceRecordCommand, bool>
{
    public async Task<bool> Handle(
        DeactivateAttendanceRecordCommand request, CancellationToken cancellationToken)
    {
        var deactivated = await repository.DeactivateAsync(request.Id);
        if (!deactivated)
            throw new KeyNotFoundException($"Attendance record with ID {request.Id} was not found.");

        return true;
    }
}

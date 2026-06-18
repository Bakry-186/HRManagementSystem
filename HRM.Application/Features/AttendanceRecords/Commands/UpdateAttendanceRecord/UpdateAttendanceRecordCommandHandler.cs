using AutoMapper;
using HRM.Application.DTOs.AttendanceRecord;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.AttendanceRecords.Commands.UpdateAttendanceRecord;

public class UpdateAttendanceRecordCommandHandler(IAttendanceRepository repository, IMapper mapper)
    : IRequestHandler<UpdateAttendanceRecordCommand, AttendanceRecordResponseDto>
{
    public async Task<AttendanceRecordResponseDto> Handle(
        UpdateAttendanceRecordCommand request, CancellationToken cancellationToken)
    {
        var attendanceRecord = await repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Attendance record with ID {request.Id} was not found.");

        mapper.Map(request.Dto, attendanceRecord);
        var updated = await repository.UpdateAsync(attendanceRecord);
        return mapper.Map<AttendanceRecordResponseDto>(updated);
    }
}

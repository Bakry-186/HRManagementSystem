using AutoMapper;
using HRM.Application.DTOs.AttendanceRecord;
using HRM.Application.Interfaces;
using MediatR;
namespace HRM.Application.Features.AttendanceRecords.Queries.GetAttendanceRecordById;

public class GetAttendanceRecordByIdQueryHandler(IAttendanceRepository repository, IMapper mapper)
    : IRequestHandler<GetAttendanceRecordByIdQuery, AttendanceRecordResponseDto>
{
    public async Task<AttendanceRecordResponseDto> Handle(GetAttendanceRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var attendanceRecord = await repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Attendance record with ID {request.Id} was not found.");

        return mapper.Map<AttendanceRecordResponseDto>(attendanceRecord);
    }
}

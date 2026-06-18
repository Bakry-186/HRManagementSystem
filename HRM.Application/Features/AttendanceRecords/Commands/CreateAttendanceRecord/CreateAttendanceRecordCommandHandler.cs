using AutoMapper;
using HRM.Application.DTOs.AttendanceRecord;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using MediatR;

namespace HRM.Application.Features.AttendanceRecords.Commands.CreateAttendanceRecord;

public class CreateAttendanceRecordCommandHandler(IAttendanceRepository repository, IEmployeeRepository employeeRepository, IMapper mapper)
    : IRequestHandler<CreateAttendanceRecordCommand, AttendanceRecordResponseDto>
{
    public async Task<AttendanceRecordResponseDto> Handle(
        CreateAttendanceRecordCommand request, CancellationToken cancellationToken)
    {
        _ = await employeeRepository.GetByIdAsync(request.Dto.EmployeeId)
            ?? throw new KeyNotFoundException($"Employee with ID {request.Dto.EmployeeId} was not found.");

        var existing = await repository.GetByEmployeeIdAndDateAsync(request.Dto.EmployeeId, request.Dto.Date);
        if (existing is not null)
            throw new InvalidOperationException($"Attendance record already exists for employee {request.Dto.EmployeeId} on {request.Dto.Date}.");

        var attendanceRecord = mapper.Map<AttendanceRecord>(request.Dto);
        var created = await repository.CreateAsync(attendanceRecord);
        return mapper.Map<AttendanceRecordResponseDto>(created);
    }
}

using HRM.Application.DTOs.AttendanceRecord;
using MediatR;

namespace HRM.Application.Features.AttendanceRecords.Commands.CreateAttendanceRecord;

public record CreateAttendanceRecordCommand(CreateAttendanceRecordDto Dto) : IRequest<AttendanceRecordResponseDto>;

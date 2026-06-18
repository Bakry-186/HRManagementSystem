using HRM.Application.DTOs.AttendanceRecord;
using MediatR;

namespace HRM.Application.Features.AttendanceRecords.Commands.UpdateAttendanceRecord;

public record UpdateAttendanceRecordCommand(Guid Id, UpdateAttendanceRecordDto Dto) : IRequest<AttendanceRecordResponseDto>;

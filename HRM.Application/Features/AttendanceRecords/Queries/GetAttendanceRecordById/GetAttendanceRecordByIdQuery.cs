using HRM.Application.DTOs.AttendanceRecord;
using MediatR;

namespace HRM.Application.Features.AttendanceRecords.Queries.GetAttendanceRecordById;

public record GetAttendanceRecordByIdQuery(Guid Id) : IRequest<AttendanceRecordResponseDto>;

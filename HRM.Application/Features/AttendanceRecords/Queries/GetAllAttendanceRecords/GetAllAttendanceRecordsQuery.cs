using HRM.Application.Common.Models;
using HRM.Application.DTOs.AttendanceRecord;
using MediatR;

namespace HRM.Application.Features.AttendanceRecords.Queries.GetAllAttendanceRecords;

public record GetAllAttendanceRecordsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<AttendanceRecordResponseDto>>;
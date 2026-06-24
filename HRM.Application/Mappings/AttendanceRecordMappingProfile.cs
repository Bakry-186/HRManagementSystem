using AutoMapper;
using HRM.Application.DTOs.AttendanceRecord;
using HRM.Domain.Entities;

namespace HRM.Application.Mappings;

public class AttendanceRecordMappingProfile : Profile
{
    public AttendanceRecordMappingProfile()
    {
        CreateMap<AttendanceRecord, AttendanceRecordResponseDto>();
        CreateMap<CreateAttendanceRecordDto, AttendanceRecord>();
        CreateMap<UpdateAttendanceRecordDto, AttendanceRecord>();
    }
}

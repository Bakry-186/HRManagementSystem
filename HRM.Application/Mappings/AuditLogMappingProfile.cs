using AutoMapper;
using HRM.Application.DTOs.AuditLog;
using HRM.Domain.Entities;

namespace HRM.Application.Mappings;

public class AuditLogMappingProfile : Profile
{
    public AuditLogMappingProfile()
    {
        CreateMap<AuditLog, AuditLogResponseDto>();
    }
}

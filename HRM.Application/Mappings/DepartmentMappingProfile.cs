using AutoMapper;
using HRM.Application.DTOs.Department;
using HRM.Domain.Entities;

namespace HRM.Application.Mappings;

public class DepartmentMappingProfile : Profile
{
    public DepartmentMappingProfile()
    {
        CreateMap<Department, DepartmentResponseDto>();
        CreateMap<CreateOrUpdateDepartmentDto, Department>();
    }
}

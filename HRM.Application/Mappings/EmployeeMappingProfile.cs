using AutoMapper;
using HRM.Application.DTOs.Employee;
using HRM.Domain.Entities;

namespace HRM.Application.Mappings;

public class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        CreateMap<Employee, EmployeeResponseDto>();
        CreateMap<Employee, EmployeeV2ResponseDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        CreateMap<CreateEmployeeDto, Employee>();
        CreateMap<UpdateEmployeeDto, Employee>();
    }
}

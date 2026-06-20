using AutoMapper;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Domain.Entities;

namespace HRM.Application.Mappings;

public class PayrollRecordMappingProfile : Profile
{
    public PayrollRecordMappingProfile()
    {
        CreateMap<PayrollRecord, PayrollRecordResponseDto>();
        CreateMap<CreatePayrollRecordDto, PayrollRecord>();
        CreateMap<UpdatePayrollRecordDto, PayrollRecord>();
        CreateMap<UpdatePayrollRecordStatusDto, PayrollRecord>();
    }
}

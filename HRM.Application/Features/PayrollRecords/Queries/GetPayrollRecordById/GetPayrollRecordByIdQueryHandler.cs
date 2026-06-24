using AutoMapper;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Queries.GetPayrollRecordById;

public class GetPayrollRecordByIdQueryHandler(IPayrollRepository repository, IMapper mapper)
    : IRequestHandler<GetPayrollRecordByIdQuery, PayrollRecordResponseDto>
{
    public async Task<PayrollRecordResponseDto> Handle(GetPayrollRecordByIdQuery request, CancellationToken cancellationToken)
    {
        var payrollRecord = await repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Payroll record with ID {request.Id} was not found.");

        return mapper.Map<PayrollRecordResponseDto>(payrollRecord);
    }
}

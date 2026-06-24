using AutoMapper;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Interfaces;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Commands.UpdatePayrollRecord;

public class UpdatePayrollRecordCommandHandler(IPayrollRepository repository, IMapper mapper)
    : IRequestHandler<UpdatePayrollRecordCommand, PayrollRecordResponseDto>
{
    public async Task<PayrollRecordResponseDto> Handle(UpdatePayrollRecordCommand request, CancellationToken cancellationToken)
    {
        var payrollRecord = await repository.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Payroll record with ID {request.Id} was not found.");

        mapper.Map(request.Dto, payrollRecord);
        payrollRecord.NetPay = payrollRecord.BasicSalary + payrollRecord.OvertimeAmount + payrollRecord.Bonus - payrollRecord.Deductions;
        var updated = await repository.UpdateAsync(payrollRecord);
        return mapper.Map<PayrollRecordResponseDto>(updated);
    }
}

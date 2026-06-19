using AutoMapper;
using HRM.Application.Constants;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Commands.CreatePayrollRecord;

public class CreatePayrollRecordCommandHandler(IPayrollRepository repository, IEmployeeRepository employeeRepository, IMapper mapper)
    : IRequestHandler<CreatePayrollRecordCommand, PayrollRecordResponseDto>
{
    public async Task<PayrollRecordResponseDto> Handle(CreatePayrollRecordCommand request, CancellationToken cancellationToken)
    {
        var payrollRecord = mapper.Map<PayrollRecord>(request.Dto);

        _ = await employeeRepository.GetByIdAsync(request.Dto.EmployeeId)
            ?? throw new KeyNotFoundException($"Employee with ID {request.Dto.EmployeeId} was not found.");

        var existing = await repository.GetByEmployeeIdAndPeriodAsync(request.Dto.EmployeeId, request.Dto.PeriodStart);
        if (existing is not null)
            throw new InvalidOperationException($"Payroll record already exists for employee {request.Dto.EmployeeId} and period starting {request.Dto.PeriodStart}.");

        payrollRecord.Status = PayrollStatus.Draft;
        payrollRecord.NetPay = payrollRecord.BasicSalary + payrollRecord.OvertimeAmount + payrollRecord.Bonus - payrollRecord.Deductions;
        var created = await repository.CreateAsync(payrollRecord);
        return mapper.Map<PayrollRecordResponseDto>(created);
    }
}

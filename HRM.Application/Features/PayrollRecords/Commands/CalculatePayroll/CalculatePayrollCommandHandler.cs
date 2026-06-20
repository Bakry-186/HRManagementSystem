using AutoMapper;
using HRM.Application.Constants;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using MediatR;

namespace HRM.Application.Features.PayrollRecords.Commands.CalculatePayroll;

public class CalculatePayrollCommandHandler(
    IPayrollRepository payrollRepository,
    IEmployeeRepository employeeRepository,
    IAttendanceRepository attendanceRepository,
    IMapper mapper)
    : IRequestHandler<CalculatePayrollCommand, PayrollRecordResponseDto>
{
    public async Task<PayrollRecordResponseDto> Handle(CalculatePayrollCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var employee = await employeeRepository.GetByIdAsync(dto.EmployeeId)
            ?? throw new KeyNotFoundException($"Employee with ID {dto.EmployeeId} was not found.");

        var existing = await payrollRepository.GetByEmployeeIdAndPeriodAsync(dto.EmployeeId, dto.PeriodStart);
        if (existing is not null)
            throw new InvalidOperationException($"Payroll record already exists for employee {dto.EmployeeId} and period starting {dto.PeriodStart}.");

        var attendanceRecords = await attendanceRepository.GetByEmployeeIdAndPeriodAsync(dto.EmployeeId, dto.PeriodStart, dto.PeriodEnd);
        if (attendanceRecords.Count == 0)
            throw new ArgumentException("Attendance records for the selected payroll period are incomplete or missing.");

        var presentCount  = attendanceRecords.Count(ar => ar.Status == AttendanceStatus.Present);
        var lateCount     = attendanceRecords.Count(ar => ar.Status == AttendanceStatus.Late);
        var halfDayCount  = attendanceRecords.Count(ar => ar.Status == AttendanceStatus.HalfDay);
        var absentCount   = attendanceRecords.Count(ar => ar.Status == AttendanceStatus.Absent);

        var effectiveDaysPresent = presentCount + lateCount + (halfDayCount * 0.5m);
        var grossPay = dto.WorkingDays > 0
            ? (employee.Salary / dto.WorkingDays) * effectiveDaysPresent
            : 0;
        var netPay = grossPay + dto.Bonus - dto.Deductions;

        var payrollRecord = new PayrollRecord
        {
            EmployeeId   = dto.EmployeeId,
            PeriodStart  = dto.PeriodStart,
            PeriodEnd    = dto.PeriodEnd,
            BasicSalary  = employee.Salary,
            OvertimeAmount = 0,
            WorkingDays  = dto.WorkingDays,
            DaysPresent  = (int)effectiveDaysPresent,
            DaysAbsent   = absentCount,
            Bonus        = dto.Bonus,
            Deductions   = dto.Deductions,
            NetPay       = netPay,
            Status       = PayrollStatus.Draft
        };

        var created = await payrollRepository.CreateAsync(payrollRecord);
        return mapper.Map<PayrollRecordResponseDto>(created);
    }
}

namespace HRM.Application.DTOs.PayrollRecord;

public class UpdatePayrollRecordDto
{
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal OvertimeAmount { get; set; }
    public int? WorkingDays { get; set; }
    public int? DaysPresent { get; set; }
    public int? DaysAbsent { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deductions { get; set; }
}

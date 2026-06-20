namespace HRM.Application.DTOs.PayrollRecord;

public class CalculatePayrollDto
{
    public Guid EmployeeId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public int WorkingDays { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deductions { get; set; }
}

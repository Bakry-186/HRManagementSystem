namespace HRM.Domain.Entities;

public class PayrollRecord : BaseEntity
{
    public required Guid EmployeeId { get; set; }
    public required DateOnly PeriodStart { get; set; }
    public required DateOnly PeriodEnd { get; set; }
    public required decimal BasicSalary { get; set; }
    public required decimal OvertimeAmount { get; set; }
    public int? WorkingDays { get; set; }
    public int? DaysPresent { get; set; }
    public int? DaysAbsent { get; set; }
    public required decimal Bonus { get; set; }
    public required decimal Deductions { get; set; }
    public required decimal NetPay { get; set; }
    public required string Status { get; set; }
    public DateOnly? PaymentDate { get; set; }
}

namespace HRM.Application.DTOs.PayrollRecord;

public class PayrollRecordResponseDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal OvertimeAmount { get; set; }
    public int? WorkingDays { get; set; }
    public int? DaysPresent { get; set; }
    public int? DaysAbsent { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetPay { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateOnly? PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

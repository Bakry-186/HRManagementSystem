using HRM.Application.Constants;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Features.PayrollRecords.Commands.CalculatePayroll;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.PayrollRecords;

public class CalculatePayrollCommandHandlerTests
{
    private readonly Mock<IPayrollRepository> _payrollRepoMock;
    private readonly Mock<IEmployeeRepository> _employeeRepoMock;
    private readonly Mock<IAttendanceRepository> _attendanceRepoMock;
    private readonly CalculatePayrollCommandHandler _handler;

    private static readonly Guid EmployeeId = Guid.NewGuid();
    private static readonly DateOnly PeriodStart = new(2026, 6, 1);
    private static readonly DateOnly PeriodEnd = new(2026, 6, 30);

    private static Employee FakeEmployee(decimal salary = 6000) => new()
    {
        Id = EmployeeId,
        FirstName = "John", LastName = "Doe", Email = "john@example.com",
        JobTitle = "Developer", HireDate = new DateOnly(2024, 1, 1),
        Salary = salary, IsActive = true,
        CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
    };

    private static AttendanceRecord MakeRecord(string status) => new()
    {
        Id = Guid.NewGuid(), EmployeeId = EmployeeId,
        Date = PeriodStart, Status = status, IsActive = true,
        CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
    };

    private static CalculatePayrollDto BaseDto(int workingDays = 20) => new()
    {
        EmployeeId = EmployeeId,
        PeriodStart = PeriodStart,
        PeriodEnd = PeriodEnd,
        WorkingDays = workingDays,
        Bonus = 0,
        Deductions = 0
    };

    private static PayrollRecord FakeCreated(decimal netPay) => new()
    {
        Id = Guid.NewGuid(), EmployeeId = EmployeeId,
        PeriodStart = PeriodStart, PeriodEnd = PeriodEnd,
        BasicSalary = 6000, OvertimeAmount = 0,
        WorkingDays = 20, DaysPresent = 20, DaysAbsent = 0,
        Bonus = 0, Deductions = 0, NetPay = netPay,
        Status = PayrollStatus.Draft, IsActive = true,
        CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
    };

    public CalculatePayrollCommandHandlerTests()
    {
        _payrollRepoMock    = new Mock<IPayrollRepository>();
        _employeeRepoMock   = new Mock<IEmployeeRepository>();
        _attendanceRepoMock = new Mock<IAttendanceRepository>();

        _handler = new CalculatePayrollCommandHandler(
            _payrollRepoMock.Object,
            _employeeRepoMock.Object,
            _attendanceRepoMock.Object,
            MappingTestHelper.CreateMapper());
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Normal case: 20 Present days, salary 6000, working days 20
    // GrossPay = (6000 / 20) × 20 = 6000
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_AllPresent_NetPayEqualsFullSalary()
    {
        var dto = BaseDto(workingDays: 20);
        var attendance = Enumerable.Range(0, 20).Select(_ => MakeRecord(AttendanceStatus.Present)).ToList();

        _employeeRepoMock.Setup(r => r.GetByIdAsync(EmployeeId)).ReturnsAsync(FakeEmployee(6000));
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart)).ReturnsAsync((PayrollRecord?)null);
        _attendanceRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart, PeriodEnd)).ReturnsAsync(attendance);
        _payrollRepoMock.Setup(r => r.CreateAsync(It.IsAny<PayrollRecord>())).ReturnsAsync(FakeCreated(6000));

        var result = await _handler.Handle(new CalculatePayrollCommand(dto), CancellationToken.None);

        Assert.Equal(6000, result.NetPay);
        Assert.Equal(PayrollStatus.Draft, result.Status);
    }

    [Fact]
    public async Task Handle_AllPresent_CallsCreateOnce()
    {
        var dto = BaseDto();
        var attendance = Enumerable.Range(0, 20).Select(_ => MakeRecord(AttendanceStatus.Present)).ToList();

        _employeeRepoMock.Setup(r => r.GetByIdAsync(EmployeeId)).ReturnsAsync(FakeEmployee());
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart)).ReturnsAsync((PayrollRecord?)null);
        _attendanceRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart, PeriodEnd)).ReturnsAsync(attendance);
        _payrollRepoMock.Setup(r => r.CreateAsync(It.IsAny<PayrollRecord>())).ReturnsAsync(FakeCreated(6000));

        await _handler.Handle(new CalculatePayrollCommand(dto), CancellationToken.None);

        _payrollRepoMock.Verify(r => r.CreateAsync(It.IsAny<PayrollRecord>()), Times.Once);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // HalfDay case: 18 Present + 2 HalfDay
    // EffectiveDays = 18 + (2 × 0.5) = 19
    // GrossPay = (6000 / 20) × 19 = 5700
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_WithHalfDays_CalculatesCorrectly()
    {
        var dto = BaseDto(workingDays: 20);
        var attendance = Enumerable.Range(0, 18).Select(_ => MakeRecord(AttendanceStatus.Present))
            .Concat(Enumerable.Range(0, 2).Select(_ => MakeRecord(AttendanceStatus.HalfDay)))
            .ToList();

        var expectedNetPay = (6000m / 20) * 19; // 5700

        _employeeRepoMock.Setup(r => r.GetByIdAsync(EmployeeId)).ReturnsAsync(FakeEmployee(6000));
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart)).ReturnsAsync((PayrollRecord?)null);
        _attendanceRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart, PeriodEnd)).ReturnsAsync(attendance);
        _payrollRepoMock.Setup(r => r.CreateAsync(It.IsAny<PayrollRecord>()))
            .ReturnsAsync((PayrollRecord pr) => pr);

        var result = await _handler.Handle(new CalculatePayrollCommand(dto), CancellationToken.None);

        Assert.Equal(expectedNetPay, result.NetPay);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // All-absent case: 20 Absent days
    // EffectiveDays = 0 → GrossPay = 0 → NetPay = 0
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_AllAbsent_NetPayIsZero()
    {
        var dto = BaseDto(workingDays: 20);
        var attendance = Enumerable.Range(0, 20).Select(_ => MakeRecord(AttendanceStatus.Absent)).ToList();

        _employeeRepoMock.Setup(r => r.GetByIdAsync(EmployeeId)).ReturnsAsync(FakeEmployee(6000));
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart)).ReturnsAsync((PayrollRecord?)null);
        _attendanceRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart, PeriodEnd)).ReturnsAsync(attendance);
        _payrollRepoMock.Setup(r => r.CreateAsync(It.IsAny<PayrollRecord>()))
            .ReturnsAsync((PayrollRecord pr) => pr);

        var result = await _handler.Handle(new CalculatePayrollCommand(dto), CancellationToken.None);

        Assert.Equal(0, result.NetPay);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Bonus and Deductions applied correctly
    // GrossPay = 6000, Bonus = 500, Deductions = 200 → NetPay = 6300
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_WithBonusAndDeductions_AppliedToNetPay()
    {
        var dto = new CalculatePayrollDto
        {
            EmployeeId = EmployeeId, PeriodStart = PeriodStart, PeriodEnd = PeriodEnd,
            WorkingDays = 20, Bonus = 500, Deductions = 200
        };
        var attendance = Enumerable.Range(0, 20).Select(_ => MakeRecord(AttendanceStatus.Present)).ToList();

        _employeeRepoMock.Setup(r => r.GetByIdAsync(EmployeeId)).ReturnsAsync(FakeEmployee(6000));
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart)).ReturnsAsync((PayrollRecord?)null);
        _attendanceRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart, PeriodEnd)).ReturnsAsync(attendance);
        _payrollRepoMock.Setup(r => r.CreateAsync(It.IsAny<PayrollRecord>()))
            .ReturnsAsync((PayrollRecord pr) => pr);

        var result = await _handler.Handle(new CalculatePayrollCommand(dto), CancellationToken.None);

        Assert.Equal(6300, result.NetPay); // 6000 + 500 - 200
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Guard cases
    // ──────────────────────────────────────────────────────────────────────────
    [Fact]
    public async Task Handle_EmployeeNotFound_ThrowsKeyNotFoundException()
    {
        _employeeRepoMock.Setup(r => r.GetByIdAsync(EmployeeId)).ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new CalculatePayrollCommand(BaseDto()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DuplicatePeriod_ThrowsInvalidOperationException()
    {
        _employeeRepoMock.Setup(r => r.GetByIdAsync(EmployeeId)).ReturnsAsync(FakeEmployee());
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart))
            .ReturnsAsync(FakeCreated(6000));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new CalculatePayrollCommand(BaseDto()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_MissingAttendance_ThrowsArgumentException()
    {
        _employeeRepoMock.Setup(r => r.GetByIdAsync(EmployeeId)).ReturnsAsync(FakeEmployee());
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart)).ReturnsAsync((PayrollRecord?)null);
        _attendanceRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart, PeriodEnd)).ReturnsAsync([]);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(new CalculatePayrollCommand(BaseDto()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_MissingAttendance_NeverCallsCreate()
    {
        _employeeRepoMock.Setup(r => r.GetByIdAsync(EmployeeId)).ReturnsAsync(FakeEmployee());
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart)).ReturnsAsync((PayrollRecord?)null);
        _attendanceRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(EmployeeId, PeriodStart, PeriodEnd)).ReturnsAsync([]);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(new CalculatePayrollCommand(BaseDto()), CancellationToken.None));

        _payrollRepoMock.Verify(r => r.CreateAsync(It.IsAny<PayrollRecord>()), Times.Never);
    }
}

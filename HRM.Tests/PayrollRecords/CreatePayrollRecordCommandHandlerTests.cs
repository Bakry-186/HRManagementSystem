using HRM.Application.Constants;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Features.PayrollRecords.Commands.CreatePayrollRecord;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.PayrollRecords;

public class CreatePayrollRecordCommandHandlerTests
{
    private readonly Mock<IPayrollRepository> _payrollRepoMock;
    private readonly Mock<IEmployeeRepository> _employeeRepoMock;
    private readonly CreatePayrollRecordCommandHandler _handler;

    private static readonly Guid EmployeeId = Guid.NewGuid();
    private static readonly DateOnly PeriodStart = new(2026, 6, 1);
    private static readonly DateOnly PeriodEnd = new(2026, 6, 30);

    private static Employee FakeEmployee => new()
    {
        Id = EmployeeId,
        FirstName = "John",
        LastName = "Doe",
        Email = "john@example.com",
        JobTitle = "Developer",
        HireDate = new DateOnly(2024, 1, 1),
        Salary = 5000,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    private static CreatePayrollRecordDto ValidDto => new()
    {
        EmployeeId = EmployeeId,
        PeriodStart = PeriodStart,
        PeriodEnd = PeriodEnd,
        BasicSalary = 5000,
        OvertimeAmount = 300,
        Bonus = 200,
        Deductions = 100
    };

    private static PayrollRecord FakeRecord(CreatePayrollRecordDto dto) => new()
    {
        Id = Guid.NewGuid(),
        EmployeeId = dto.EmployeeId,
        PeriodStart = dto.PeriodStart,
        PeriodEnd = dto.PeriodEnd,
        BasicSalary = dto.BasicSalary,
        OvertimeAmount = dto.OvertimeAmount,
        Bonus = dto.Bonus,
        Deductions = dto.Deductions,
        NetPay = dto.BasicSalary + dto.OvertimeAmount + dto.Bonus - dto.Deductions,
        Status = PayrollStatus.Draft,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public CreatePayrollRecordCommandHandlerTests()
    {
        _payrollRepoMock = new Mock<IPayrollRepository>();
        _employeeRepoMock = new Mock<IEmployeeRepository>();
        _handler = new CreatePayrollRecordCommandHandler(
            _payrollRepoMock.Object,
            _employeeRepoMock.Object,
            MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsResponseDto()
    {
        var dto = ValidDto;
        _employeeRepoMock.Setup(r => r.GetByIdAsync(dto.EmployeeId)).ReturnsAsync(FakeEmployee);
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(dto.EmployeeId, dto.PeriodStart)).ReturnsAsync((PayrollRecord?)null);
        _payrollRepoMock.Setup(r => r.CreateAsync(It.IsAny<PayrollRecord>())).ReturnsAsync(FakeRecord(dto));

        var result = await _handler.Handle(new CreatePayrollRecordCommand(dto), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(dto.EmployeeId, result.EmployeeId);
        Assert.Equal(PayrollStatus.Draft, result.Status);
    }

    [Fact]
    public async Task Handle_ValidRequest_NetPayCalculatedCorrectly()
    {
        var dto = ValidDto;
        var expectedNetPay = dto.BasicSalary + dto.OvertimeAmount + dto.Bonus - dto.Deductions;

        _employeeRepoMock.Setup(r => r.GetByIdAsync(dto.EmployeeId)).ReturnsAsync(FakeEmployee);
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(dto.EmployeeId, dto.PeriodStart)).ReturnsAsync((PayrollRecord?)null);
        _payrollRepoMock.Setup(r => r.CreateAsync(It.IsAny<PayrollRecord>())).ReturnsAsync(FakeRecord(dto));

        var result = await _handler.Handle(new CreatePayrollRecordCommand(dto), CancellationToken.None);

        Assert.Equal(expectedNetPay, result.NetPay);
    }

    [Fact]
    public async Task Handle_ValidRequest_StatusIsDraft()
    {
        var dto = ValidDto;
        _employeeRepoMock.Setup(r => r.GetByIdAsync(dto.EmployeeId)).ReturnsAsync(FakeEmployee);
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(dto.EmployeeId, dto.PeriodStart)).ReturnsAsync((PayrollRecord?)null);
        _payrollRepoMock.Setup(r => r.CreateAsync(It.IsAny<PayrollRecord>())).ReturnsAsync(FakeRecord(dto));

        var result = await _handler.Handle(new CreatePayrollRecordCommand(dto), CancellationToken.None);

        Assert.Equal(PayrollStatus.Draft, result.Status);
    }

    [Fact]
    public async Task Handle_EmployeeNotFound_ThrowsKeyNotFoundException()
    {
        var dto = ValidDto;
        _employeeRepoMock.Setup(r => r.GetByIdAsync(dto.EmployeeId)).ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new CreatePayrollRecordCommand(dto), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmployeeNotFound_NeverCallsCreate()
    {
        var dto = ValidDto;
        _employeeRepoMock.Setup(r => r.GetByIdAsync(dto.EmployeeId)).ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new CreatePayrollRecordCommand(dto), CancellationToken.None));

        _payrollRepoMock.Verify(r => r.CreateAsync(It.IsAny<PayrollRecord>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DuplicatePeriod_ThrowsInvalidOperationException()
    {
        var dto = ValidDto;
        _employeeRepoMock.Setup(r => r.GetByIdAsync(dto.EmployeeId)).ReturnsAsync(FakeEmployee);
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(dto.EmployeeId, dto.PeriodStart)).ReturnsAsync(FakeRecord(dto));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new CreatePayrollRecordCommand(dto), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DuplicatePeriod_NeverCallsCreate()
    {
        var dto = ValidDto;
        _employeeRepoMock.Setup(r => r.GetByIdAsync(dto.EmployeeId)).ReturnsAsync(FakeEmployee);
        _payrollRepoMock.Setup(r => r.GetByEmployeeIdAndPeriodAsync(dto.EmployeeId, dto.PeriodStart)).ReturnsAsync(FakeRecord(dto));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new CreatePayrollRecordCommand(dto), CancellationToken.None));

        _payrollRepoMock.Verify(r => r.CreateAsync(It.IsAny<PayrollRecord>()), Times.Never);
    }
}

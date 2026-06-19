using HRM.Application.Constants;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Features.PayrollRecords.Commands.UpdatePayrollRecord;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.PayrollRecords;

public class UpdatePayrollRecordCommandHandlerTests
{
    private readonly Mock<IPayrollRepository> _repositoryMock;
    private readonly UpdatePayrollRecordCommandHandler _handler;

    private static PayrollRecord ExistingRecord => new()
    {
        Id = Guid.NewGuid(),
        EmployeeId = Guid.NewGuid(),
        PeriodStart = new DateOnly(2026, 6, 1),
        PeriodEnd = new DateOnly(2026, 6, 30),
        BasicSalary = 5000,
        OvertimeAmount = 0,
        Bonus = 0,
        Deductions = 0,
        NetPay = 5000,
        Status = PayrollStatus.Draft,
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    public UpdatePayrollRecordCommandHandlerTests()
    {
        _repositoryMock = new Mock<IPayrollRepository>();
        _handler = new UpdatePayrollRecordCommandHandler(_repositoryMock.Object, MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_ExistingRecord_ReturnsUpdatedDto()
    {
        var record = ExistingRecord;
        var dto = new UpdatePayrollRecordDto
        {
            PeriodStart = record.PeriodStart,
            PeriodEnd = record.PeriodEnd,
            BasicSalary = 6000,
            OvertimeAmount = 500,
            Bonus = 300,
            Deductions = 200
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(record.Id)).ReturnsAsync(record);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<PayrollRecord>())).ReturnsAsync(record);

        var result = await _handler.Handle(new UpdatePayrollRecordCommand(record.Id, dto), CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_Update_RecalculatesNetPay()
    {
        var record = ExistingRecord;
        var dto = new UpdatePayrollRecordDto
        {
            PeriodStart = record.PeriodStart,
            PeriodEnd = record.PeriodEnd,
            BasicSalary = 6000,
            OvertimeAmount = 500,
            Bonus = 300,
            Deductions = 200
        };
        var expectedNetPay = dto.BasicSalary + dto.OvertimeAmount + dto.Bonus - dto.Deductions;

        _repositoryMock.Setup(r => r.GetByIdAsync(record.Id)).ReturnsAsync(record);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<PayrollRecord>()))
            .ReturnsAsync((PayrollRecord pr) => pr);

        var result = await _handler.Handle(new UpdatePayrollRecordCommand(record.Id, dto), CancellationToken.None);

        Assert.Equal(expectedNetPay, result.NetPay);
    }

    [Fact]
    public async Task Handle_ExistingRecord_CallsUpdateOnce()
    {
        var record = ExistingRecord;
        var dto = new UpdatePayrollRecordDto { PeriodStart = record.PeriodStart, PeriodEnd = record.PeriodEnd, BasicSalary = 5000 };

        _repositoryMock.Setup(r => r.GetByIdAsync(record.Id)).ReturnsAsync(record);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<PayrollRecord>())).ReturnsAsync(record);

        await _handler.Handle(new UpdatePayrollRecordCommand(record.Id, dto), CancellationToken.None);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PayrollRecord>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentRecord_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((PayrollRecord?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new UpdatePayrollRecordCommand(id, new UpdatePayrollRecordDto()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NonExistentRecord_NeverCallsUpdate()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((PayrollRecord?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new UpdatePayrollRecordCommand(id, new UpdatePayrollRecordDto()), CancellationToken.None));

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PayrollRecord>()), Times.Never);
    }
}

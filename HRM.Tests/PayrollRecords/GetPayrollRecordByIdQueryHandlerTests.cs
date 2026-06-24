using HRM.Application.Constants;
using HRM.Application.Features.PayrollRecords.Queries.GetPayrollRecordById;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.PayrollRecords;

public class GetPayrollRecordByIdQueryHandlerTests
{
    private readonly Mock<IPayrollRepository> _repositoryMock;
    private readonly GetPayrollRecordByIdQueryHandler _handler;

    public GetPayrollRecordByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IPayrollRepository>();
        _handler = new GetPayrollRecordByIdQueryHandler(_repositoryMock.Object, MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_ExistingRecord_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var record = new PayrollRecord
        {
            Id = id,
            EmployeeId = Guid.NewGuid(),
            PeriodStart = new DateOnly(2026, 6, 1),
            PeriodEnd = new DateOnly(2026, 6, 30),
            BasicSalary = 5000,
            OvertimeAmount = 300,
            Bonus = 200,
            Deductions = 100,
            NetPay = 5400,
            Status = PayrollStatus.Draft,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(record);

        var result = await _handler.Handle(new GetPayrollRecordByIdQuery(id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(5400, result.NetPay);
        Assert.Equal(PayrollStatus.Draft, result.Status);
    }

    [Fact]
    public async Task Handle_NonExistentRecord_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((PayrollRecord?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new GetPayrollRecordByIdQuery(id), CancellationToken.None));
    }
}

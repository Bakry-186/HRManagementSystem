using HRM.Application.Constants;
using HRM.Application.DTOs.PayrollRecord;
using HRM.Application.Features.PayrollRecords.Commands.UpdatePayrollStatus;
using HRM.Application.Interfaces;
using Moq;

namespace HRM.Tests.PayrollRecords;

public class UpdatePayrollStatusCommandHandlerTests
{
    private readonly Mock<IPayrollRepository> _repositoryMock;
    private readonly UpdatePayrollStatusCommandHandler _handler;

    public UpdatePayrollStatusCommandHandlerTests()
    {
        _repositoryMock = new Mock<IPayrollRepository>();
        _handler = new UpdatePayrollStatusCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidStatus_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var dto = new UpdatePayrollRecordStatusDto { Status = PayrollStatus.Approved };

        _repositoryMock.Setup(r => r.UpdateStatusAsync(id, dto.Status)).ReturnsAsync(true);

        var result = await _handler.Handle(new UpdatePayrollStatusCommand(id, dto), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ValidStatus_CallsUpdateStatusOnce()
    {
        var id = Guid.NewGuid();
        var dto = new UpdatePayrollRecordStatusDto { Status = PayrollStatus.PendingApproval };

        _repositoryMock.Setup(r => r.UpdateStatusAsync(id, dto.Status)).ReturnsAsync(true);

        await _handler.Handle(new UpdatePayrollStatusCommand(id, dto), CancellationToken.None);

        _repositoryMock.Verify(r => r.UpdateStatusAsync(id, dto.Status), Times.Once);
    }
}

using HRM.Application.Features.PayrollRecords.Commands.DeactivatePayrollRecord;
using HRM.Application.Interfaces;
using Moq;

namespace HRM.Tests.PayrollRecords;

public class DeactivatePayrollRecordCommandHandlerTests
{
    private readonly Mock<IPayrollRepository> _repositoryMock;
    private readonly DeactivatePayrollRecordCommandHandler _handler;

    public DeactivatePayrollRecordCommandHandlerTests()
    {
        _repositoryMock = new Mock<IPayrollRepository>();
        _handler = new DeactivatePayrollRecordCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingRecord_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(id)).ReturnsAsync(true);

        var result = await _handler.Handle(new DeactivatePayrollRecordCommand(id), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ExistingRecord_CallsDeactivateOnce()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(id)).ReturnsAsync(true);

        await _handler.Handle(new DeactivatePayrollRecordCommand(id), CancellationToken.None);

        _repositoryMock.Verify(r => r.DeactivateAsync(id), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentRecord_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(id)).ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new DeactivatePayrollRecordCommand(id), CancellationToken.None));
    }
}

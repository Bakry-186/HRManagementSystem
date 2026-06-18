using HRM.Application.Features.AttendanceRecords.Commands.DeactivateAttendanceRecord;
using HRM.Application.Interfaces;
using Moq;

namespace HRM.Tests.AttendanceRecords;

public class DeactivateAttendanceRecordCommandHandlerTests
{
    private readonly Mock<IAttendanceRepository> _repositoryMock;
    private readonly DeactivateAttendanceRecordCommandHandler _handler;

    public DeactivateAttendanceRecordCommandHandlerTests()
    {
        _repositoryMock = new Mock<IAttendanceRepository>();
        _handler = new DeactivateAttendanceRecordCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingRecord_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(id)).ReturnsAsync(true);

        var result = await _handler.Handle(new DeactivateAttendanceRecordCommand(id), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ExistingRecord_CallsDeactivateOnce()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(id)).ReturnsAsync(true);

        await _handler.Handle(new DeactivateAttendanceRecordCommand(id), CancellationToken.None);

        _repositoryMock.Verify(r => r.DeactivateAsync(id), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentRecord_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(id)).ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new DeactivateAttendanceRecordCommand(id), CancellationToken.None));
    }
}

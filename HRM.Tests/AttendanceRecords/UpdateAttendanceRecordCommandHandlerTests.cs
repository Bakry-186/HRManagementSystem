using HRM.Application.DTOs.AttendanceRecord;
using HRM.Application.Features.AttendanceRecords.Commands.UpdateAttendanceRecord;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.AttendanceRecords;

public class UpdateAttendanceRecordCommandHandlerTests
{
    private readonly Mock<IAttendanceRepository> _repositoryMock;
    private readonly UpdateAttendanceRecordCommandHandler _handler;

    public UpdateAttendanceRecordCommandHandlerTests()
    {
        _repositoryMock = new Mock<IAttendanceRepository>();
        _handler = new UpdateAttendanceRecordCommandHandler(_repositoryMock.Object, MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_ExistingRecord_ReturnsUpdatedDto()
    {
        var id = Guid.NewGuid();
        var existing = new AttendanceRecord
        {
            Id = id,
            EmployeeId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CheckInTime = new TimeOnly(9, 0),
            Status = "Present",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var dto = new UpdateAttendanceRecordDto
        {
            CheckOutTime = new TimeOnly(17, 0),
            Status = "Present"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AttendanceRecord>())).ReturnsAsync(existing);

        var result = await _handler.Handle(new UpdateAttendanceRecordCommand(id, dto), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task Handle_ExistingRecord_CallsUpdateOnce()
    {
        var id = Guid.NewGuid();
        var existing = new AttendanceRecord
        {
            Id = id,
            EmployeeId = Guid.NewGuid(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            Status = "Present",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AttendanceRecord>())).ReturnsAsync(existing);

        await _handler.Handle(new UpdateAttendanceRecordCommand(id, new UpdateAttendanceRecordDto { Status = "Late" }), CancellationToken.None);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AttendanceRecord>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentRecord_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AttendanceRecord?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new UpdateAttendanceRecordCommand(id, new UpdateAttendanceRecordDto()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NonExistentRecord_NeverCallsUpdate()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AttendanceRecord?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new UpdateAttendanceRecordCommand(id, new UpdateAttendanceRecordDto()), CancellationToken.None));

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AttendanceRecord>()), Times.Never);
    }
}

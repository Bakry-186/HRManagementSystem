using HRM.Application.Features.AttendanceRecords.Queries.GetAttendanceRecordById;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.AttendanceRecords;

public class GetAttendanceRecordByIdQueryHandlerTests
{
    private readonly Mock<IAttendanceRepository> _repositoryMock;
    private readonly GetAttendanceRecordByIdQueryHandler _handler;

    public GetAttendanceRecordByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IAttendanceRepository>();
        _handler = new GetAttendanceRecordByIdQueryHandler(_repositoryMock.Object, MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_ExistingRecord_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var record = new AttendanceRecord
        {
            Id = id,
            EmployeeId = employeeId,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            CheckInTime = new TimeOnly(9, 0),
            Status = "Present",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(record);

        var result = await _handler.Handle(new GetAttendanceRecordByIdQuery(id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(employeeId, result.EmployeeId);
        Assert.Equal("Present", result.Status);
    }

    [Fact]
    public async Task Handle_NonExistentRecord_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((AttendanceRecord?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new GetAttendanceRecordByIdQuery(id), CancellationToken.None));
    }
}

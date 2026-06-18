using HRM.Application.DTOs.AttendanceRecord;
using HRM.Application.Features.AttendanceRecords.Commands.CreateAttendanceRecord;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.AttendanceRecords;

public class CreateAttendanceRecordCommandHandlerTests
{
    private readonly Mock<IAttendanceRepository> _repositoryMock;
    private readonly CreateAttendanceRecordCommandHandler _handler;

    private static readonly Guid EmployeeId = Guid.NewGuid();
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    public CreateAttendanceRecordCommandHandlerTests()
    {
        _repositoryMock = new Mock<IAttendanceRepository>();
        _handler = new CreateAttendanceRecordCommandHandler(_repositoryMock.Object, MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsAttendanceRecordResponseDto()
    {
        var dto = new CreateAttendanceRecordDto
        {
            EmployeeId = EmployeeId,
            Date = Today,
            CheckInTime = new TimeOnly(9, 0),
            Status = "Present"
        };
        var command = new CreateAttendanceRecordCommand(dto);

        _repositoryMock
            .Setup(r => r.GetByEmployeeIdAndDateAsync(dto.EmployeeId, dto.Date))
            .ReturnsAsync((AttendanceRecord?)null);

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<AttendanceRecord>()))
            .ReturnsAsync(new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                EmployeeId = dto.EmployeeId,
                Date = dto.Date,
                CheckInTime = dto.CheckInTime,
                Status = dto.Status,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(dto.EmployeeId, result.EmployeeId);
        Assert.Equal(dto.Date, result.Date);
        Assert.Equal(dto.Status, result.Status);
    }

    [Fact]
    public async Task Handle_ValidRequest_CallsRepositoryCreateOnce()
    {
        var dto = new CreateAttendanceRecordDto
        {
            EmployeeId = EmployeeId,
            Date = Today,
            Status = "Present"
        };

        _repositoryMock
            .Setup(r => r.GetByEmployeeIdAndDateAsync(dto.EmployeeId, dto.Date))
            .ReturnsAsync((AttendanceRecord?)null);

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<AttendanceRecord>()))
            .ReturnsAsync(new AttendanceRecord { Id = Guid.NewGuid(), EmployeeId = dto.EmployeeId, Date = dto.Date, Status = dto.Status });

        await _handler.Handle(new CreateAttendanceRecordCommand(dto), CancellationToken.None);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<AttendanceRecord>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateRecord_ThrowsInvalidOperationException()
    {
        var dto = new CreateAttendanceRecordDto { EmployeeId = EmployeeId, Date = Today, Status = "Present" };

        _repositoryMock
            .Setup(r => r.GetByEmployeeIdAndDateAsync(dto.EmployeeId, dto.Date))
            .ReturnsAsync(new AttendanceRecord { Id = Guid.NewGuid(), EmployeeId = dto.EmployeeId, Date = dto.Date, Status = "Present" });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new CreateAttendanceRecordCommand(dto), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DuplicateRecord_NeverCallsCreate()
    {
        var dto = new CreateAttendanceRecordDto { EmployeeId = EmployeeId, Date = Today, Status = "Present" };

        _repositoryMock
            .Setup(r => r.GetByEmployeeIdAndDateAsync(dto.EmployeeId, dto.Date))
            .ReturnsAsync(new AttendanceRecord { Id = Guid.NewGuid(), EmployeeId = dto.EmployeeId, Date = dto.Date, Status = "Present" });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new CreateAttendanceRecordCommand(dto), CancellationToken.None));

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<AttendanceRecord>()), Times.Never);
    }
}

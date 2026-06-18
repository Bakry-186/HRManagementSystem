using HRM.Application.Common.Models;
using HRM.Application.Features.AttendanceRecords.Queries.GetAllAttendanceRecords;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.AttendanceRecords;

public class GetAllAttendanceRecordsQueryHandlerTests
{
    private readonly Mock<IAttendanceRepository> _repositoryMock;
    private readonly GetAllAttendanceRecordsQueryHandler _handler;

    public GetAllAttendanceRecordsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IAttendanceRepository>();
        _handler = new GetAllAttendanceRecordsQueryHandler(_repositoryMock.Object, MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_WithRecords_ReturnsMappedPagedResult()
    {
        var records = new List<AttendanceRecord>
        {
            new() { Id = Guid.NewGuid(), EmployeeId = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow), Status = "Present", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), EmployeeId = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.UtcNow), Status = "Late",    IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(new PagedResult<AttendanceRecord>
            {
                Items = records,
                TotalCount = records.Count,
                PageNumber = 1,
                PageSize = 10
            });

        var result = await _handler.Handle(new GetAllAttendanceRecordsQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyPagedResult()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(new PagedResult<AttendanceRecord>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 10
            });

        var result = await _handler.Handle(new GetAllAttendanceRecordsQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Handle_WithRecords_CallsGetAllOnce()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(new PagedResult<AttendanceRecord> { Items = [], TotalCount = 0, PageNumber = 1, PageSize = 10 });

        await _handler.Handle(new GetAllAttendanceRecordsQuery(1, 10), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetAllAsync(1, 10), Times.Once);
    }
}

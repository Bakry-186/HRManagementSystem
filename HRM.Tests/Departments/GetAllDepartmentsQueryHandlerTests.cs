using HRM.Application.Common.Models;
using HRM.Application.Features.Departments.Queries.GetAllDepartments;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.Departments;

public class GetAllDepartmentsQueryHandlerTests
{
    private readonly Mock<IDepartmentRepository> _repositoryMock;
    private readonly GetAllDepartmentsQueryHandler _handler;

    public GetAllDepartmentsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IDepartmentRepository>();
        _handler = new GetAllDepartmentsQueryHandler(_repositoryMock.Object, MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_WithDepartments_ReturnsPagedResult()
    {
        var departments = new List<Department>
        {
            new() { Id = Guid.NewGuid(), Name = "Engineering", IsActive = true },
            new() { Id = Guid.NewGuid(), Name = "HR", IsActive = true }
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(new PagedResult<Department>
            {
                Items = departments,
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            });

        var result = await _handler.Handle(new GetAllDepartmentsQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(1, result.PageNumber);
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyPagedResult()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(1, 10))
            .ReturnsAsync(new PagedResult<Department>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 10
            });

        var result = await _handler.Handle(new GetAllDepartmentsQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectPaginationMetadata()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync(2, 5))
            .ReturnsAsync(new PagedResult<Department>
            {
                Items = [new() { Id = Guid.NewGuid(), Name = "Legal" }],
                TotalCount = 11,
                PageNumber = 2,
                PageSize = 5
            });

        var result = await _handler.Handle(new GetAllDepartmentsQuery(2, 5), CancellationToken.None);

        Assert.Equal(2, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(11, result.TotalCount);
    }
}

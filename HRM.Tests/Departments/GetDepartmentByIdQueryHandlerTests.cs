using HRM.Application.Features.Departments.Queries.GetDepartmentById;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.Departments;

public class GetDepartmentByIdQueryHandlerTests
{
    private readonly Mock<IDepartmentRepository> _repositoryMock;
    private readonly GetDepartmentByIdQueryHandler _handler;

    public GetDepartmentByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IDepartmentRepository>();
        _handler = new GetDepartmentByIdQueryHandler(_repositoryMock.Object, MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_ExistingDepartment_ReturnsDepartmentResponseDto()
    {
        var departmentId = Guid.NewGuid();
        var department = new Department
        {
            Id = departmentId,
            Name = "Finance",
            Description = "Finance team",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);

        var result = await _handler.Handle(new GetDepartmentByIdQuery(departmentId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(departmentId, result.Id);
        Assert.Equal("Finance", result.Name);
    }

    [Fact]
    public async Task Handle_NonExistingDepartment_ThrowsKeyNotFoundException()
    {
        var departmentId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new GetDepartmentByIdQuery(departmentId), CancellationToken.None));
    }
}

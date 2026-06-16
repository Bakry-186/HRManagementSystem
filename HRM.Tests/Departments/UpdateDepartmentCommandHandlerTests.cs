using HRM.Application.DTOs.Department;
using HRM.Application.Features.Departments.Commands.UpdateDepartment;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.Departments;

public class UpdateDepartmentCommandHandlerTests
{
    private readonly Mock<IDepartmentRepository> _repositoryMock;
    private readonly UpdateDepartmentCommandHandler _handler;

    public UpdateDepartmentCommandHandlerTests()
    {
        _repositoryMock = new Mock<IDepartmentRepository>();
        _handler = new UpdateDepartmentCommandHandler(_repositoryMock.Object, MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_ExistingDepartment_ReturnsUpdatedDto()
    {
        var departmentId = Guid.NewGuid();
        var existing = new Department { Id = departmentId, Name = "Old Name", IsActive = true };
        var dto = new CreateOrUpdateDepartmentDto { Name = "New Name", Description = "Updated" };
        var command = new UpdateDepartmentCommand(departmentId, dto);

        _repositoryMock.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Department>())).ReturnsAsync(existing);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingDepartment_ThrowsKeyNotFoundException()
    {
        var departmentId = Guid.NewGuid();
        var command = new UpdateDepartmentCommand(departmentId, new CreateOrUpdateDepartmentDto { Name = "X" });

        _repositoryMock.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NonExistingDepartment_NeverCallsUpdate()
    {
        var departmentId = Guid.NewGuid();
        var command = new UpdateDepartmentCommand(departmentId, new CreateOrUpdateDepartmentDto { Name = "X" });

        _repositoryMock.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Department>()), Times.Never);
    }
}

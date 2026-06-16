using HRM.Application.Features.Departments.Commands.DeactivateDepartment;
using HRM.Application.Interfaces;
using Moq;

namespace HRM.Tests.Departments;

public class DeactivateDepartmentCommandHandlerTests
{
    private readonly Mock<IDepartmentRepository> _repositoryMock;
    private readonly DeactivateDepartmentCommandHandler _handler;

    public DeactivateDepartmentCommandHandlerTests()
    {
        _repositoryMock = new Mock<IDepartmentRepository>();
        _handler = new DeactivateDepartmentCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingDepartment_ReturnsTrue()
    {
        var departmentId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(departmentId)).ReturnsAsync(true);

        var result = await _handler.Handle(new DeactivateDepartmentCommand(departmentId), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_NonExistingDepartment_ThrowsKeyNotFoundException()
    {
        var departmentId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(departmentId)).ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(new DeactivateDepartmentCommand(departmentId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExistingDepartment_CallsDeactivateOnce()
    {
        var departmentId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(departmentId)).ReturnsAsync(true);

        await _handler.Handle(new DeactivateDepartmentCommand(departmentId), CancellationToken.None);

        _repositoryMock.Verify(r => r.DeactivateAsync(departmentId), Times.Once);
    }
}

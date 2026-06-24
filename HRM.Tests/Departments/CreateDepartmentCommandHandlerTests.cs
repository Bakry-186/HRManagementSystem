using HRM.Application.DTOs.Department;
using HRM.Application.Features.Departments.Commands.CreateDepartment;
using HRM.Application.Interfaces;
using HRM.Domain.Entities;
using HRM.Tests.Common;
using Moq;

namespace HRM.Tests.Departments;

public class CreateDepartmentCommandHandlerTests
{
    private readonly Mock<IDepartmentRepository> _repositoryMock;
    private readonly CreateDepartmentCommandHandler _handler;

    public CreateDepartmentCommandHandlerTests()
    {
        _repositoryMock = new Mock<IDepartmentRepository>();
        _handler = new CreateDepartmentCommandHandler(_repositoryMock.Object, MappingTestHelper.CreateMapper());
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsDepartmentResponseDto()
    {
        var dto = new CreateDepartmentDto { Name = "Engineering", Description = "Tech team" };
        var command = new CreateDepartmentCommand(dto);

        var createdDepartment = new Department
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Department>()))
            .ReturnsAsync(createdDepartment);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Description, result.Description);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task Handle_ValidRequest_CallsRepositoryCreateOnce()
    {
        var dto = new CreateDepartmentDto { Name = "HR" };
        var command = new CreateDepartmentCommand(dto);

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Department>()))
            .ReturnsAsync(new Department { Id = Guid.NewGuid(), Name = dto.Name });

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Department>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateName_ThrowsInvalidOperationException()
    {
        var dto = new CreateDepartmentDto { Name = "Engineering" };
        _repositoryMock.Setup(r => r.NameExistsAsync(dto.Name)).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new CreateDepartmentCommand(dto), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_DuplicateName_NeverCallsCreate()
    {
        var dto = new CreateDepartmentDto { Name = "Engineering" };
        _repositoryMock.Setup(r => r.NameExistsAsync(dto.Name)).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new CreateDepartmentCommand(dto), CancellationToken.None));

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Department>()), Times.Never);
    }
}

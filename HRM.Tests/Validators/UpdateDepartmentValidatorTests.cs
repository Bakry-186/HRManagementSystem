using FluentValidation.TestHelper;
using HRM.Application.DTOs.Department;
using HRM.Application.Validators.Department;

namespace HRM.Tests.Validators;

public class UpdateDepartmentValidatorTests
{
    private readonly UpdateDepartmentValidator _validator = new();

    [Fact]
    public void Should_Pass_When_AllFieldsAreValid()
    {
        var dto = new UpdateDepartmentDto { Name = "Operations", Description = "Ops team" };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Pass_When_DescriptionIsNull()
    {
        var dto = new UpdateDepartmentDto { Name = "Finance", Description = null };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_NameExceeds100Characters()
    {
        var dto = new UpdateDepartmentDto { Name = new string('A', 101) };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Fail_When_DescriptionExceeds250Characters()
    {
        var dto = new UpdateDepartmentDto { Name = "Finance", Description = new string('A', 251) };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Description);
    }
}

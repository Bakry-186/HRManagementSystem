using FluentValidation.TestHelper;
using HRM.Application.DTOs.Department;
using HRM.Application.Validators.Department;

namespace HRM.Tests.Validators;

public class CreateDepartmentValidatorTests
{
    private readonly CreateDepartmentValidator _validator = new();

    [Fact]
    public void Should_Pass_When_AllFieldsAreValid()
    {
        var dto = new CreateDepartmentDto { Name = "Engineering", Description = "Tech team" };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Pass_When_DescriptionIsNull()
    {
        var dto = new CreateDepartmentDto { Name = "HR", Description = null };
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_When_NameIsEmpty(string name)
    {
        var dto = new CreateDepartmentDto { Name = name };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Fail_When_NameExceeds100Characters()
    {
        var dto = new CreateDepartmentDto { Name = new string('A', 101) };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Fail_When_DescriptionExceeds250Characters()
    {
        var dto = new CreateDepartmentDto { Name = "Engineering", Description = new string('A', 251) };
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Description);
    }
}

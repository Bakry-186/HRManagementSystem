using FluentValidation.TestHelper;
using HRM.Application.DTOs.Employee;
using HRM.Application.Validators.Employee;

namespace HRM.Tests.Validators;

public class UpdateEmployeeValidatorTests
{
    private readonly UpdateEmployeeValidator _validator = new();

    private static UpdateEmployeeDto ValidDto() => new()
    {
        FirstName = "Jane",
        LastName = "Smith",
        Email = "jane.smith@company.com",
        JobTitle = "Senior Engineer",
        Salary = 8000,
        HireDate = DateOnly.FromDateTime(DateTime.UtcNow)
    };

    [Fact]
    public void Should_Pass_When_AllFieldsAreValid()
    {
        var result = _validator.TestValidate(ValidDto());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_FirstNameExceeds100Characters()
    {
        var dto = ValidDto();
        dto.FirstName = new string('A', 101);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Fail_When_LastNameExceeds100Characters()
    {
        var dto = ValidDto();
        dto.LastName = new string('A', 101);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    public void Should_Fail_When_EmailFormatIsInvalid(string email)
    {
        var dto = ValidDto();
        dto.Email = email;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Fail_When_JobTitleExceeds150Characters()
    {
        var dto = ValidDto();
        dto.JobTitle = new string('A', 151);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.JobTitle);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Should_Fail_When_SalaryIsZeroOrNegative(decimal salary)
    {
        var dto = ValidDto();
        dto.Salary = salary;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Salary);
    }

    [Fact]
    public void Should_Fail_When_HireDateIsInTheFuture()
    {
        var dto = ValidDto();
        dto.HireDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.HireDate);
    }
}

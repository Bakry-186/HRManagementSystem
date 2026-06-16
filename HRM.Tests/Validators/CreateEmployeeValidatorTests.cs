using FluentValidation.TestHelper;
using HRM.Application.DTOs.Employee;
using HRM.Application.Validators.Employee;

namespace HRM.Tests.Validators;

public class CreateEmployeeValidatorTests
{
    private readonly CreateEmployeeValidator _validator = new();

    private static CreateEmployeeDto ValidDto() => new()
    {
        FirstName = "John",
        LastName = "Doe",
        Email = "john.doe@company.com",
        JobTitle = "Engineer",
        Salary = 5000,
        HireDate = DateOnly.FromDateTime(DateTime.UtcNow)
    };

    [Fact]
    public void Should_Pass_When_AllFieldsAreValid()
    {
        var result = _validator.TestValidate(ValidDto());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_When_FirstNameIsEmpty(string firstName)
    {
        var dto = ValidDto();
        dto.FirstName = firstName;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Should_Fail_When_FirstNameExceeds100Characters()
    {
        var dto = ValidDto();
        dto.FirstName = new string('A', 101);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_When_LastNameIsEmpty(string lastName)
    {
        var dto = ValidDto();
        dto.LastName = lastName;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Should_Fail_When_LastNameExceeds100Characters()
    {
        var dto = ValidDto();
        dto.LastName = new string('A', 101);
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    [InlineData("missing@")]
    public void Should_Fail_When_EmailIsInvalid(string email)
    {
        var dto = ValidDto();
        dto.Email = email;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_When_JobTitleIsEmpty(string jobTitle)
    {
        var dto = ValidDto();
        dto.JobTitle = jobTitle;
        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.JobTitle);
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
    [InlineData(-1)]
    [InlineData(-500)]
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

    [Fact]
    public void Should_Pass_When_PhoneNumberIsNull()
    {
        var dto = ValidDto();
        dto.PhoneNumber = null;
        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }
}

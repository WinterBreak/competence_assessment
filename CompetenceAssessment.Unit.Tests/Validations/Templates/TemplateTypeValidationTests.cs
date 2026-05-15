using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TemplateTypeValidationTests
{
    private readonly TemplateTypeValidation _rule = new();

    [Fact]
    public async Task Validate_Should_Add_Error_When_Template_Type_Is_None()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Type = TemplateType.None
        };

        await _rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }

    [Theory]
    [InlineData(TemplateType.Test)]
    [InlineData(TemplateType.Survey)]
    public async Task Validate_Should_Not_Add_Error_When_Template_Type_Is_Valid(TemplateType type)
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Type = type
        };

        await _rule.ValidateAsync(template, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Throw_When_Template_Is_Null()
    {
        var errors = new ValidationErrors();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _rule.ValidateAsync(null!, errors));
    }

    [Fact]
    public async Task Validate_Should_Throw_When_ValidationErrors_Is_Null()
    {
        var template = new ITemplate
        {
            Id = 1,
            Type = TemplateType.Survey
        };

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _rule.ValidateAsync(template, null!));
    }
}
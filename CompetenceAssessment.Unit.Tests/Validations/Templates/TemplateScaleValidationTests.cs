using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TemplateScaleValidationTests
{
    private readonly TemplateScaleValidation _rule = new();

    [Fact]
    public async Task Validate_Should_Add_Error_When_Scale_Is_Not_Set()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
        };

        await _rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }

    [Theory]
    [InlineData(ScaleType.FivePointScale)]
    [InlineData(ScaleType.TenPointScale)]
    public async Task Validate_Should_Not_Add_Error_When_Scale_Is_Set(ScaleType scale)
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Scale = scale
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
            Scale = ScaleType.FivePointScale
        };

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _rule.ValidateAsync(template, null!));
    }
}
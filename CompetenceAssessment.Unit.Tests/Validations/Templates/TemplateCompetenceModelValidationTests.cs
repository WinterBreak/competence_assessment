using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TemplateCompetenceModelValidationTests
{
    private readonly TemplateCompetenceModelValidation _rule = new();

    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Competence_Model_Id_Is_Set()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            CompetenceModelId = 1
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
            CompetenceModelId = 1
        };

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _rule.ValidateAsync(template, null!));
    }
}
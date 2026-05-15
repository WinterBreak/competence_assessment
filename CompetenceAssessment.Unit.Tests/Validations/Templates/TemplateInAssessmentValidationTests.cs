using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TemplateInAssessmentValidationTests
{
    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Template_Is_Not_Used_In_Assessment()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1
        };

        var queries = new Mock<ITemplateValidationQueries>();

        queries.Setup(q => q.IsUsedInAssessmentAsync(
                template.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var rule = new TemplateInAssessmentValidation(queries.Object);

        await rule.ValidateAsync(template, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Template_Is_Used_In_Assessment()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1
        };

        var queries = new Mock<ITemplateValidationQueries>();

        queries.Setup(q => q.IsUsedInAssessmentAsync(
                template.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var rule = new TemplateInAssessmentValidation(queries.Object);

        await rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }
}
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TemplateExistenceValidationTests
{
    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Template_Exists()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1
        };

        var queries = new Mock<ITemplateValidationQueries>();

        queries.Setup(q => q.IsExistAsync(
                template.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var rule = new TemplateExistenceValidation(queries.Object);

        await rule.ValidateAsync(template, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Template_Does_Not_Exist()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1
        };

        var queries = new Mock<ITemplateValidationQueries>();

        queries.Setup(q => q.IsExistAsync(
                template.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var rule = new TemplateExistenceValidation(queries.Object);

        await rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }
}
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class AssessmentExistenceValidationTests
{
    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Assessment_Exists()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Id = 1
        };

        var queries = new Mock<IAssessmentValidationQueries>();

        queries.Setup(q => q.IsExistAsync(
                assessment.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var rule = new AssessmentExistenceValidation(queries.Object);

        await rule.ValidateAsync(assessment, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Assessment_Does_Not_Exist()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Id = 1
        };

        var queries = new Mock<IAssessmentValidationQueries>();

        queries.Setup(q => q.IsExistAsync(
                assessment.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var rule = new AssessmentExistenceValidation(queries.Object);

        await rule.ValidateAsync(assessment, errors);

        Assert.True(errors.HasErrors);
    }
}
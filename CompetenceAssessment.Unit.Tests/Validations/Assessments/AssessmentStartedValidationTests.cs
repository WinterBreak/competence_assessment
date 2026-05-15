using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class AssessmentStartedValidationTests
{
    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Assessment_Not_Started()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Candidate = new AssessmentParticipant { Id = 1 },
            Template = new ITemplate { Id = 10 },
            Type = AssessmentType.Testing
        };

        var queries = new Mock<IAssessmentValidationQueries>();

        queries.Setup(q => q.IsAssessmentStartedAsync(
                assessment.Candidate.Id,
                assessment.Template.Id,
                assessment.Type,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var rule = new AssessmentStartedValidation(queries.Object);

        await rule.ValidateAsync(assessment, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Assessment_Already_Started()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Candidate = new AssessmentParticipant { Id = 1 },
            Template = new ITemplate { Id = 10 },
            Type = AssessmentType.Testing
        };

        var queries = new Mock<IAssessmentValidationQueries>();

        queries.Setup(q => q.IsAssessmentStartedAsync(
                assessment.Candidate.Id,
                assessment.Template.Id,
                assessment.Type,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var rule = new AssessmentStartedValidation(queries.Object);

        await rule.ValidateAsync(assessment, errors);

        Assert.True(errors.HasErrors);
    }
}
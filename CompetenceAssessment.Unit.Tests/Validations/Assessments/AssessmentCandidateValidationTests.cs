using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class AssessmentCandidateValidationTests
{
    private readonly AssessmentCandidateValidation _rule = new();

    [Fact]
    public async Task Validate_Should_Add_Error_When_Candidate_Id_Is_Null()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Candidate = new AssessmentParticipant
            {
                Id = 0
            }
        };

        await _rule.ValidateAsync(assessment, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Candidate_Id_Is_Set()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Candidate = new AssessmentParticipant
            {
                Id = 1
            }
        };

        await _rule.ValidateAsync(assessment, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Throw_When_Assessment_Is_Null()
    {
        var errors = new ValidationErrors();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _rule.ValidateAsync(null!, errors));
    }

    [Fact]
    public async Task Validate_Should_Throw_When_ValidationErrors_Is_Null()
    {
        var assessment = new Assessment
        {
            Candidate = new AssessmentParticipant
            {
                Id = 1
            }
        };

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _rule.ValidateAsync(assessment, null!));
    }
}
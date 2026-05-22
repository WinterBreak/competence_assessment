using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class AssessmentResultValidationTests
{
    private readonly AssessmentResultValidation _rule = new();

    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Assessment_Type_Is_Testing()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Type = AssessmentType.Testing,
            Results = new List<AssessmentResult>
            {
                new AssessmentResult
                {
                    Answer = null
                }
            }
        };

        await _rule.ValidateAsync(assessment, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Answer_Is_Null()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Type = AssessmentType.Survey,
            Results = new List<AssessmentResult>
            {
                new AssessmentResult
                {
                    Answer = null!
                }
            }
        };

        await _rule.ValidateAsync(assessment, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Answer_Is_Empty()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Type = AssessmentType.Survey,
            Results = new List<AssessmentResult>
            {
                new AssessmentResult
                {
                    Answer = string.Empty
                }
            }
        };

        await _rule.ValidateAsync(assessment, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_All_Answers_Are_Valid()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Type = AssessmentType.Survey,
            Results = new List<AssessmentResult>
            {
                new AssessmentResult
                {
                    Answer = "Yes"
                },
                new AssessmentResult
                {
                    Answer = "No"
                }
            }
        };

        await _rule.ValidateAsync(assessment, errors);

        Assert.False(errors.HasErrors);
    }
}
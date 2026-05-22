using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class AssessmentInspectorsValidationTests
{
    private readonly AssessmentInspectorsValidation _rule = new();

    [Fact]
    public async Task Validate_Should_Add_Error_When_Inspectors_Is_Null()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Type = AssessmentType.Testing,
            Inspectors = null!
        };

        await _rule.ValidateAsync(assessment, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Inspectors_Is_Empty()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Type = AssessmentType.Testing,
            Inspectors = new List<AssessmentParticipant>()
        };

        await _rule.ValidateAsync(assessment, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_360_Degrees_And_Less_Than_Two_Inspectors()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Type = AssessmentType._360Degrees_,
            Inspectors = new List<AssessmentParticipant>
            {
                new AssessmentParticipant { Id = 1 }
            }
        };

        await _rule.ValidateAsync(assessment, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Valid_Inspectors_For_360()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Type = AssessmentType._360Degrees_,
            Inspectors = new List<AssessmentParticipant>
            {
                new AssessmentParticipant { Id = 1 },
                new AssessmentParticipant { Id = 2 }
            }
        };

        await _rule.ValidateAsync(assessment, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Valid_Inspectors_For_Other_Type()
    {
        var errors = new ValidationErrors();

        var assessment = new Assessment
        {
            Type = AssessmentType.Testing,
            Inspectors = new List<AssessmentParticipant>
            {
                new AssessmentParticipant { Id = 1 }
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
            Type = AssessmentType.Testing,
            Inspectors = new List<AssessmentParticipant>()
        };

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _rule.ValidateAsync(assessment, null!));
    }
}
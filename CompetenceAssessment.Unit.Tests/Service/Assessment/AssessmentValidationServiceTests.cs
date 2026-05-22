using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;
using Moq;
using Xunit;

namespace CompetenceAssessment.Tests.Application.Assessment;

public class AssessmentValidationServiceTests
{
    private readonly Mock<IAssessmentValidationQueries> _queriesMock;
    private readonly AssessmentValidationService _service;

    public AssessmentValidationServiceTests()
    {
        _queriesMock = new Mock<IAssessmentValidationQueries>();
        _service = new AssessmentValidationService(_queriesMock.Object);
    }

    [Fact]
    public async Task ValidateCreatingAssessmentAsync_Should_Return_NoErrors_When_Assessment_Is_Valid()
    {
        var assessment = CreateValidAssessment();

        SetupAssessmentStarted(false);

        var result = await _service.ValidateCreatingAssessmentAsync(assessment);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateCreatingAssessmentAsync_Should_Return_Error_When_Template_Is_Empty()
    {
        var assessment = CreateValidAssessment();
        assessment.Template = new ITemplate();

        SetupAssessmentStarted(false);

        var result = await _service.ValidateCreatingAssessmentAsync(assessment);

        Assert.True(result.HasErrors);
        Assert.True(result.ErrorValues.ContainsKey(ErrorsConfg.EMPTY_FIELD));
    }

    [Fact]
    public async Task ValidateCreatingAssessmentAsync_Should_Return_Error_When_Candidate_Is_Empty()
    {
        var assessment = CreateValidAssessment();
        assessment.Candidate = new AssessmentParticipant();

        SetupAssessmentStarted(false);

        var result = await _service.ValidateCreatingAssessmentAsync(assessment);

        Assert.True(result.HasErrors);
        Assert.True(result.ErrorValues.ContainsKey(ErrorsConfg.EMPTY_FIELD));
    }

    [Fact]
    public async Task ValidateCreatingAssessmentAsync_Should_Return_Error_When_Inspectors_Are_Empty()
    {
        var assessment = CreateValidAssessment();
        assessment.Inspectors = [];

        SetupAssessmentStarted(false);

        var result = await _service.ValidateCreatingAssessmentAsync(assessment);

        Assert.True(result.HasErrors);
        Assert.True(result.ErrorValues.ContainsKey(ErrorsConfg.EMPTY_FIELD));
    }

    [Fact]
    public async Task ValidateCreatingAssessmentAsync_Should_Return_Error_When_Assessment_Already_Started()
    {
        var assessment = CreateValidAssessment();

        SetupAssessmentStarted(true);

        var result = await _service.ValidateCreatingAssessmentAsync(assessment);

        Assert.True(result.HasErrors);
        Assert.True(result.ErrorValues.ContainsKey("AssessmentStarted"));
    }

    [Fact]
    public async Task ValidateUpdatingAssessmentAsync_Should_Return_NoErrors_When_Results_Are_Valid()
    {
        var assessment = CreateValidAssessment();

        var result = await _service.ValidateUpdatingAssessmentAsync(assessment);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateUpdatingAssessmentAsync_Should_Return_Error_When_Answer_Is_Empty()
    {
        var assessment = CreateValidAssessment();

        assessment.Type = AssessmentType.Survey;

        assessment.Results =
        [
            new AssessmentResult
            {
                Answer = string.Empty,
                Score = 5,
                Task = new ITask
                {
                    Id = 1
                }
            }
        ];

        var result = await _service.ValidateUpdatingAssessmentAsync(assessment);

        Assert.True(result.HasErrors);
        Assert.True(result.ErrorValues.ContainsKey(ErrorsConfg.EMPTY_FIELD));
    }

    [Fact]
    public async Task ValidateExistenceAsync_Should_Return_NoErrors_When_Assessment_Exists()
    {
        SetupAssessmentExists(true);

        var result = await _service.ValidateExistenceAsync(1);

        Assert.False(result.HasErrors);
    }

    [Fact]
    public async Task ValidateExistenceAsync_Should_Return_Error_When_Assessment_Not_Exists()
    {
        SetupAssessmentExists(false);

        var result = await _service.ValidateExistenceAsync(1);

        Assert.True(result.HasErrors);
        Assert.True(result.ErrorValues.ContainsKey(ErrorsConfg.NON_EXISTENT_ENTITY));
    }

    private void SetupAssessmentExists(bool exists)
    {
        _queriesMock
            .Setup(q => q.IsExistAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists);
    }

    private void SetupAssessmentStarted(bool started)
    {
        _queriesMock
            .Setup(q => q.IsAssessmentStartedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<AssessmentType>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(started);
    }

    private static Domain.Assessment.Assessment CreateValidAssessment()
    {
        return new Domain.Assessment.Assessment
        {
            Id = 1,
            Candidate = new AssessmentParticipant
            {
                Id = 1,
                FullName = "Candidate"
            },
            Template = new ITemplate
            {
                Id = 1
            },
            Type = AssessmentType.Testing,
            Inspectors =
            [
                new AssessmentParticipant
                {
                    Id = 2,
                    FullName = "Inspector"
                }
            ],
            Results =
            [
                new AssessmentResult
                {
                    Answer = "Answer",
                    Score = 5,
                    Task = new ITask
                    {
                        Id = 1,
                        Type = TaskType.TestQuestion
                    }
                }
            ]
        };
    }
}
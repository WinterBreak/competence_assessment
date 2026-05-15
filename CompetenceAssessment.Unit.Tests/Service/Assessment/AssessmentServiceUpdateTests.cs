using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Assessments;

public class AssessmentServiceUpdateTests
{
    [Fact]
    public async Task UpdateAssessment_Should_Save_When_Valid()
    {
        var repository = new Mock<IAssessmentRepository>();
        var validation = new Mock<IAssessmentValidationService>();

        var existing = new Assessment
        {
            Id = 1,
            Results = new List<AssessmentResult>
            {
                new AssessmentResult
                {
                    Id = 1,
                    Task = new ITask
                    {
                        Id = 1,
                        Type = TaskType.TestQuestion,
                        Answers = new List<Answer>
                        {
                            new Answer("A", true)
                        }
                    },
                    Answer = "A",
                    Score = 10
                }
            }
        };

        repository
            .Setup(r => r.GetAssessmentAsync(It.IsAny<AssessmentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        validation
            .Setup(v => v.ValidateExistenceAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationErrors());

        validation
            .Setup(v => v.ValidateUpdatingAssessmentAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationErrors());

        var service = new AssessmentService(repository.Object, validation.Object);

        var command = new UpdateAssessmentCommand(
            assessmentId: 1,
            answers: new Dictionary<int, string> { { 1, "A" } },
            scores: new Dictionary<int, int>(),
            comments: new Dictionary<int, string>(),
            comment: "ok"
        );

        var result = await service.UpdateAssessmentAsync(command);

        Assert.False(result.HasErrors);

        repository.Verify(r => r.UpdateAssessmentAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveAllChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAssessment_Should_Stop_When_Existence_Fails()
    {
        var repository = new Mock<IAssessmentRepository>();
        var validation = new Mock<IAssessmentValidationService>();

        var errors = new ValidationErrors();
        errors.AddMainError("not found");

        validation
            .Setup(v => v.ValidateExistenceAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errors);

        var service = new AssessmentService(repository.Object, validation.Object);

        var command = new UpdateAssessmentCommand(
            assessmentId: 1,
            answers: new Dictionary<int, string>(),
            scores: new Dictionary<int, int>(),
            comments: new Dictionary<int, string>(),
            comment: "ok"
        );

        var result = await service.UpdateAssessmentAsync(command);

        Assert.True(result.HasErrors);

        repository.Verify(r => r.UpdateAssessmentAsync(It.IsAny<Assessment>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
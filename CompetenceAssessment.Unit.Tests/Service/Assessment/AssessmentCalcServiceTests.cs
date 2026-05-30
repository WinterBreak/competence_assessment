using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.DTO;
using Moq;
using Xunit;

namespace CompetenceAssessment.Tests.Application.Assessment;

public class AssessmentCalcServiceTests
{
    private readonly Mock<IAssessmentRepository> _repositoryMock;
    private readonly AssessmentCalcService _service;

    public AssessmentCalcServiceTests()
    {
        _repositoryMock = new Mock<IAssessmentRepository>();
        _service = new AssessmentCalcService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CalculateAsync_Should_Throw_When_Assessment_Not_Found()
    {
        _repositoryMock
            .Setup(x => x.GetAssessmentAsync(It.IsAny<AssessmentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Assessment.Assessment)null);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.CalculateAsync(1));
    }

    [Fact]
    public async Task CalculateAsync_Should_Throw_When_Assessment_Type_Is_Invalid()
    {
        var assessment = CreateAssessment((AssessmentType)999);

        _repositoryMock
            .Setup(x => x.GetAssessmentAsync(It.IsAny<AssessmentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CalculateAsync(1));
    }

    private static Domain.Assessment.Assessment CreateAssessment(AssessmentType type)
    {
        return new Domain.Assessment.Assessment
        {
            Id = 1,
            Type = type,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(1),
            State = AssessmentState.Completed,

            Candidate = new AssessmentParticipant
            {
                Id = 1,
                FullName = "John",
            },

            Inspectors =
            [
                new AssessmentParticipant
                {
                    Id = 2,
                    FullName = "Jane",
                }
            ],

            Template = new FakeTemplate
            {
                Id = 1,
                Name = "Template",
                Description = "Description"
            },

            Results =
            [
                new AssessmentResult
                {
                    Id = 1,
                    Score = 5,
                    Task = new ITask {
                        Id = 1,
                        Text = "test",
                        Type = TaskType.SurveyQuestion
                        }
                }
            ]
        };
    }

    private class FakeTemplate : ITemplate
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
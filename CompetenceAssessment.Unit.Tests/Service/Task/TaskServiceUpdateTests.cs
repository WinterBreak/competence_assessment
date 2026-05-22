using CompetenceAssessment.Application.Assessment.Templates;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Tasks;

public class TaskServiceUpdateTests
{
    [Fact]
    public async Task UpdateTask_Should_Update_And_Save_When_Valid()
    {
        var repository = new Mock<ITaskRepository>();
        var queries = new Mock<ITaskValidationQueries>();

        var existingTask = new ITask
        {
            Id = 1,
            Text = "old",
            Type = TaskType.SurveyQuestion,
            Answers = new List<Answer>()
        };

        queries.Setup(r => r.IsTaskExistAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        repository.Setup(r => r.GetTaskAsync(It.IsAny<TaskQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);

        var service = new TaskService(repository.Object, queries.Object);

        var command = new UpdateTaskCommand(
            1,
            "new text",
            new Dictionary<string, bool> { },
            TaskType.SurveyQuestion
        );

        var result = await service.UpdateTaskAsync(command);

        Assert.False(result.HasErrors);

        repository.Verify(r => r.UpdateTaskAsync(existingTask, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveAllChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_Should_Stop_When_Validation_Fails_On_Existence()
    {
        var repository = new Mock<ITaskRepository>();
        var queries = new Mock<ITaskValidationQueries>();

        var service = new TaskService(repository.Object, queries.Object);

        queries.Setup(q => q.IsTaskExistAsync(
                It.IsAny<string>(),
                It.IsAny<TaskType>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new UpdateTaskCommand(
            1,
            "text",
            new Dictionary<string, bool> { { "A", true } },
            TaskType.TestQuestion
        );

        var result = await service.UpdateTaskAsync(command);

        Assert.True(result.HasErrors);

        repository.Verify(r => r.UpdateTaskAsync(It.IsAny<ITask>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
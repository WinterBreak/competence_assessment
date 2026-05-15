using CompetenceAssessment.Application.Assessment.Templates;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Tasks;

public class TaskServiceCreateTests
{
    [Fact]
    public async Task CreateTask_Should_Save_Task_When_Valid()
    {
        var repository = new Mock<ITaskRepository>();
        var queries = new Mock<ITaskValidationQueries>();

        var service = new TaskService(repository.Object, queries.Object);

        var command = new CreateTaskCommand(
            "text",
            TaskType.TestQuestion,
            new Dictionary<string, bool> { { "A", true } }
        );

        var result = await service.CreateTaskAsync(command);

        Assert.False(result.HasErrors);

        repository.Verify(r => r.AddTaskAsync(It.IsAny<ITask>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveAllChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTask_Should_Not_Save_When_Validation_Fails()
    {
        var repository = new Mock<ITaskRepository>();
        var queries = new Mock<ITaskValidationQueries>();

        var service = new TaskService(repository.Object, queries.Object);

        var command = new CreateTaskCommand(
            "", 
            TaskType.None,
            new Dictionary<string, bool>()
        );

        var result = await service.CreateTaskAsync(command);

        Assert.True(result.HasErrors);

        repository.Verify(r => r.AddTaskAsync(It.IsAny<ITask>(), It.IsAny<CancellationToken>()), Times.Never);
        repository.Verify(r => r.SaveAllChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
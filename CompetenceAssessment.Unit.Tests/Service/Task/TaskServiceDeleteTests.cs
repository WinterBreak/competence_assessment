using CompetenceAssessment.Application.Assessment.Templates;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Tasks;

public class TaskServiceDeleteTests
{
    [Fact]
    public async Task DeleteTask_Should_Remove_When_Valid()
    {
        var repository = new Mock<ITaskRepository>();
        var queries = new Mock<ITaskValidationQueries>();

        queries.Setup(q => q.IsTaskExistAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new TaskService(repository.Object, queries.Object);

        var command = new DeleteTaskCommand(1);

        var result = await service.DeleteTaskAsync(command);

        Assert.False(result.HasErrors);

        repository.Verify(r => r.RemoveTaskAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveAllChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_Should_Not_Delete_When_Invalid()
    {
        var repository = new Mock<ITaskRepository>();
        var queries = new Mock<ITaskValidationQueries>();

        queries.Setup(q => q.IsTaskExistAsync(
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new TaskService(repository.Object, queries.Object);

        var command = new DeleteTaskCommand(1);

        var result = await service.DeleteTaskAsync(command);

        Assert.True(result.HasErrors);

        repository.Verify(r => r.RemoveTaskAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
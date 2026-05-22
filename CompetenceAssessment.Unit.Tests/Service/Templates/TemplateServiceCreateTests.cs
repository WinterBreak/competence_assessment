using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Templates;

public class TemplateServiceCreateTests
{
    [Fact]
    public async Task CreateTemplate_Should_Save_When_Valid()
    {
        var repository = new Mock<ITemplateRepository>();
        var taskRepository = new Mock<ITaskRepository>();
        var validation = new Mock<ITemplateValidationService>();

        validation
            .Setup(v => v.ValidateCreatingTemplateAsync(It.IsAny<ITemplate>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationErrors());

        taskRepository
            .Setup(r => r.GetTasksAsync(It.IsAny<TaskQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ITask>
            {
                new ITask { Id = 1, Type = TaskType.TestQuestion },
                new ITask { Id = 2, Type = TaskType.TestQuestion },
                new ITask { Id = 3, Type = TaskType.TestQuestion },
                new ITask { Id = 4, Type = TaskType.TestQuestion },
                new ITask { Id = 5, Type = TaskType.TestQuestion }
            });

        var service = new TemplateService(repository.Object, taskRepository.Object, validation.Object);

        var command = new CreateTemplateCommand(
            "template",
            TemplateType.Survey,
            ScaleType.FivePointScale,
            1,
            new Dictionary<int, decimal>
            {
                { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }
            },
            new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 1 } }
            });

        var result = await service.CreateTemplateAsync(command);

        Assert.False(result.HasErrors);

        repository.Verify(r => r.AddTemplateAsync(It.IsAny<ITemplate>(), It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveAllChanges(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTemplate_Should_Not_Save_When_Validation_Fails()
    {
        var repository = new Mock<ITemplateRepository>();
        var taskRepository = new Mock<ITaskRepository>();
        var validation = new Mock<ITemplateValidationService>();
        
        var errors = new ValidationErrors();
        errors.AddMainError("error");

        taskRepository
            .Setup(r => r.GetTasksAsync(It.IsAny<TaskQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ITask>
            {
                new ITask { Id = 1, Type = TaskType.TestQuestion },
                new ITask { Id = 2, Type = TaskType.TestQuestion },
                new ITask { Id = 3, Type = TaskType.TestQuestion },
                new ITask { Id = 4, Type = TaskType.TestQuestion },
                new ITask { Id = 5, Type = TaskType.TestQuestion }
            });
        
        validation
            .Setup(v => v.ValidateCreatingTemplateAsync(It.IsAny<ITemplate>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errors);

        var service = new TemplateService(repository.Object, taskRepository.Object, validation.Object);

        var command = new CreateTemplateCommand(
            "template",
            TemplateType.Survey,
            ScaleType.FivePointScale,
            1,
            new Dictionary<int, decimal>
            {
                { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }
            },
            new Dictionary<int, List<int>>
            {
                { 1, new List<int> { 1 } }
            });

        var result = await service.CreateTemplateAsync(command);

        Assert.True(result.HasErrors);

        repository.Verify(r => r.AddTemplateAsync(It.IsAny<ITemplate>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
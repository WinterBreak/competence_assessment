using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Templates;

public class TemplateServiceUpdateTests
{
    [Fact]
    public async Task UpdateTemplate_Should_Save_When_Valid()
    {
        var repository = new Mock<ITemplateRepository>();
        var taskRepository = new Mock<ITaskRepository>();
        var validation = new Mock<ITemplateValidationService>();

        var existing = new ITemplate
        {
            Id = 1,
            Weights = new List<TemplateWeight>()
        };

        repository
            .Setup(r => r.GetTemplateAsync(It.IsAny<TemplateQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        validation
            .Setup(v => v.ValidateExistenceAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationErrors());

        validation
            .Setup(v => v.ValidateUpdatingTemplateAsync(It.IsAny<ITemplate>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationErrors());

        taskRepository
            .Setup(r => r.GetTasksAsync(It.IsAny<TaskQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ITask>
            {
                new ITask { Id = 1, Type = TaskType.TestQuestion }
            });

        var service = new TemplateService(repository.Object, taskRepository.Object, validation.Object);

        var command = new UpdateTemplateCommand(
            1,
            "new",
            TemplateType.Survey,
            ScaleType.FivePointScale,
            1,
            new Dictionary<int, decimal> { { 1, 1 } },
            new Dictionary<int, List<int>> { { 1, new List<int> { 1 } } });

        var result = await service.UpdateTemplateAsync(command);

        Assert.False(result.HasErrors);

        repository.Verify(r => r.UpdateTemplateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveAllChanges(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTemplate_Should_Stop_When_Entity_Not_Exists()
    {
        var repository = new Mock<ITemplateRepository>();
        var taskRepository = new Mock<ITaskRepository>();
        var validation = new Mock<ITemplateValidationService>();

        var errors = new ValidationErrors();
        errors.AddMainError("not found");

        validation
            .Setup(v => v.ValidateExistenceAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errors);

        var service = new TemplateService(repository.Object, taskRepository.Object, validation.Object);

        var command = new UpdateTemplateCommand(
            1,
            "new",
            TemplateType.Survey,
            ScaleType.FivePointScale,
            1,
            new Dictionary<int, decimal>(),
            new Dictionary<int, List<int>>());

        var result = await service.UpdateTemplateAsync(command);

        Assert.True(result.HasErrors);

        repository.Verify(r => r.UpdateTemplateAsync(It.IsAny<ITemplate>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
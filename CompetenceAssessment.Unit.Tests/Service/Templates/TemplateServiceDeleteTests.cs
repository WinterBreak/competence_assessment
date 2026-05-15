using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Services.Templates;

public class TemplateServiceDeleteTests
{
    [Fact]
    public async Task DeleteTemplate_Should_Remove_When_Valid()
    {
        var repository = new Mock<ITemplateRepository>();
        var taskRepository = new Mock<ITaskRepository>();
        var validation = new Mock<ITemplateValidationService>();

        validation
            .Setup(v => v.ValidateDeletingTemplateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationErrors());

        var service = new TemplateService(repository.Object, taskRepository.Object, validation.Object);

        var command = new DeleteTemplateCommand(1);

        var result = await service.DeleteTemplateAsync(command);

        Assert.False(result.HasErrors);

        repository.Verify(r => r.RemoveTemplateAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        repository.Verify(r => r.SaveAllChanges(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTemplate_Should_Not_Delete_When_Invalid()
    {
        var repository = new Mock<ITemplateRepository>();
        var taskRepository = new Mock<ITaskRepository>();
        var validation = new Mock<ITemplateValidationService>();

        var errors = new ValidationErrors();
        errors.AddMainError("error");

        validation
            .Setup(v => v.ValidateDeletingTemplateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errors);

        var service = new TemplateService(repository.Object, taskRepository.Object, validation.Object);

        var command = new DeleteTemplateCommand(1);

        var result = await service.DeleteTemplateAsync(command);

        Assert.True(result.HasErrors);

        repository.Verify(r => r.RemoveTemplateAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
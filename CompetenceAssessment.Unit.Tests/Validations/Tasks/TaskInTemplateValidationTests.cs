using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TaskInTemplateValidationTests
{
    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Task_Is_Not_Used_In_Template()
    {
        var errors = new ValidationErrors();

        var task = new ITask
        {
            Id = 1
        };

        var queries = new Mock<ITaskValidationQueries>();

        queries.Setup(q => q.IsUsedInTemplateAsync(
                task.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var rule = new TaskInTemplateValidation(queries.Object);

        await rule.ValidateAsync(task, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Task_Is_Used_In_Template()
    {
        var errors = new ValidationErrors();

        var task = new ITask
        {
            Id = 1
        };

        var queries = new Mock<ITaskValidationQueries>();

        queries.Setup(q => q.IsUsedInTemplateAsync(
                task.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var rule = new TaskInTemplateValidation(queries.Object);

        await rule.ValidateAsync(task, errors);

        Assert.True(errors.HasErrors);
    }
}
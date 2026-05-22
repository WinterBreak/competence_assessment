using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TaskTextValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_Should_Add_Error_When_Text_Is_Empty(string? text)
    {
        var errors = new ValidationErrors();

        var task = new ITask
        {
            Id = 1,
            Text = text!,
            Type = TaskType.TestQuestion
        };

        var queries = new Mock<ITaskValidationQueries>();

        queries.Setup(q => q.IsTaskExistAsync(
                It.IsAny<string>(),
                It.IsAny<TaskType>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var rule = new TaskTextValidation(queries.Object);

        await rule.ValidateAsync(task, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Task_Text_Already_Exists()
    {
        var errors = new ValidationErrors();

        var task = new ITask
        {
            Id = 1,
            Text = "Question",
            Type = TaskType.TestQuestion
        };

        var queries = new Mock<ITaskValidationQueries>();

        queries.Setup(q => q.IsTaskExistAsync(
                task.Text,
                task.Type,
                task.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var rule = new TaskTextValidation(queries.Object);

        await rule.ValidateAsync(task, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Text_Is_Valid_And_Unique()
    {
        var errors = new ValidationErrors();

        var task = new ITask
        {
            Id = 1,
            Text = "Question",
            Type = TaskType.TestQuestion
        };

        var queries = new Mock<ITaskValidationQueries>();

        queries.Setup(q => q.IsTaskExistAsync(
                task.Text,
                task.Type,
                task.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var rule = new TaskTextValidation(queries.Object);

        await rule.ValidateAsync(task, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Throw_When_Task_Is_Null()
    {
        var errors = new ValidationErrors();

        var queries = new Mock<ITaskValidationQueries>();

        var rule = new TaskTextValidation(queries.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            rule.ValidateAsync(null!, errors));
    }

    [Fact]
    public async Task Validate_Should_Throw_When_ValidationErrors_Is_Null()
    {
        var task = new ITask
        {
            Id = 1,
            Text = "Question",
            Type = TaskType.TestQuestion
        };

        var queries = new Mock<ITaskValidationQueries>();

        var rule = new TaskTextValidation(queries.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            rule.ValidateAsync(task, null!));
    }
}
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TaskTypeValidationTests
{
    private readonly TaskTypeValidation _rule = new();

    [Fact]
    public async Task Validate_Should_Add_Error_When_Task_Type_Is_None()
    {
        var errors = new ValidationErrors();

        var task = new ITask
        {
            Type = TaskType.None
        };

        await _rule.ValidateAsync(task, errors);

        Assert.True(errors.HasErrors);
    }

    [Theory]
    [InlineData(TaskType.TestQuestion)]
    public async Task Validate_Should_Not_Add_Error_When_Task_Type_Is_Valid(TaskType taskType)
    {
        var errors = new ValidationErrors();

        var task = new ITask
        {
            Type = taskType
        };

        await _rule.ValidateAsync(task, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Throw_When_Task_Is_Null()
    {
        var errors = new ValidationErrors();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _rule.ValidateAsync(null!, errors));
    }

    [Fact]
    public async Task Validate_Should_Throw_When_ValidationErrors_Is_Null()
    {
        var task = new ITask
        {
            Type = TaskType.TestQuestion
        };

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _rule.ValidateAsync(task, null!));
    }
}
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TaskAnswerValidationTests
{
    private readonly TaskAnswerValidation _rule = new();

    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_TestQuestion_Has_Answers_And_Correct_Answer()
    {
        var errors = new ValidationErrors();
        var task = new ITask(
            "Question",
            TaskType.TestQuestion,
            new List<Answer>
            {
                new("Answer 1", false),
                new("Answer 2", true)
            });
        
        await _rule.ValidateAsync(task, errors);
        Assert.False(errors.HasErrors);
    }

    [Theory]
    [InlineData(TaskType.SurveyQuestion)]
    [InlineData(TaskType.OpenQuestion)]
    public async Task Validate_Should_Add_Error_When_Non_TestQuestion_Has_Answers(TaskType taskType)
    {
        var errors = new ValidationErrors();
        var task = new ITask(
            "Question",
            taskType,
            new List<Answer>
            {
                new("Answer", true)
            });
        
        await _rule.ValidateAsync(task, errors);
        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Answers_Are_Null()
    {
        var errors = new ValidationErrors();
        var task = new ITask
        {
            Text = "Question",
            Type = TaskType.TestQuestion,
            Answers = null!
        };
        
        await _rule.ValidateAsync(task, errors);
        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Answers_Are_Empty()
    {
        var errors = new ValidationErrors();

        var task = new ITask(
            "Question",
            TaskType.TestQuestion,
            []);
        
        await _rule.ValidateAsync(task, errors);
        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_No_Correct_Answers_Exist()
    {
        var errors = new ValidationErrors();

        var task = new ITask(
            "Question",
            TaskType.TestQuestion,
            new List<Answer>
            {
                new("Answer 1", false),
                new("Answer 2", false)
            });
        
        await _rule.ValidateAsync(task, errors);
        Assert.True(errors.HasErrors);
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
        var task = new ITask(
            "Question",
            TaskType.TestQuestion,
            new List<Answer>
            {
                new("Answer", true)
            });
        
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _rule.ValidateAsync(task, null!));
    }
}
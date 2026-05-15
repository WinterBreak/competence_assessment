using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TemplateTasksValidationTests
{
    private readonly TemplateTasksValidation _rule = new();

    [Fact]
    public async Task Validate_Should_Add_Error_When_Weights_Is_Null()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Type = TemplateType.Survey,
        };

        await _rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Weights_Is_Empty()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Type = TemplateType.Survey,
            Weights = new List<TemplateWeight>()
        };

        await _rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Less_Than_Min_Count()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Type = TemplateType.Test,
            Weights = new List<TemplateWeight>
            {
                CreateWeight(1, TaskType.TestQuestion),
                CreateWeight(2, TaskType.TestQuestion),
                CreateWeight(3, TaskType.TestQuestion),
                CreateWeight(4, TaskType.TestQuestion)
            }
        };

        await _rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Has_Wrong_Task_Types_For_Survey()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Type = TemplateType.Survey,
            Weights = new List<TemplateWeight>
            {
                CreateWeight(1, TaskType.SurveyQuestion),
                CreateWeight(2, TaskType.TestQuestion),
                CreateWeight(3, TaskType.SurveyQuestion),
                CreateWeight(4, TaskType.SurveyQuestion),
                CreateWeight(5, TaskType.SurveyQuestion)
            }
        };

        await _rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Task_Duplicates_Exist()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Type = TemplateType.Test,
            Weights = new List<TemplateWeight>
            {
                CreateWeight(1, TaskType.TestQuestion),
                CreateWeight(1, TaskType.TestQuestion),
                CreateWeight(2, TaskType.TestQuestion),
                CreateWeight(3, TaskType.TestQuestion),
                CreateWeight(4, TaskType.TestQuestion)
            }
        };

        await _rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Template_Is_Valid()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Type = TemplateType.Test,
            Weights = new List<TemplateWeight>
            {
                CreateWeight(1, TaskType.TestQuestion),
                CreateWeight(2, TaskType.TestQuestion),
                CreateWeight(3, TaskType.TestQuestion),
                CreateWeight(4, TaskType.TestQuestion),
                CreateWeight(5, TaskType.TestQuestion)
            }
        };

        await _rule.ValidateAsync(template, errors);

        Assert.False(errors.HasErrors);
    }

    private static TemplateWeight CreateWeight(int taskId, TaskType type)
    {
        var task = new ITask
        {
            Id = taskId,
            Type = type
        };

        return new TemplateWeight(task, 1, 1);
    }
}
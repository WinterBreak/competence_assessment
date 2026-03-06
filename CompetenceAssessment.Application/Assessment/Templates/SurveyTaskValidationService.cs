using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class SurveyTaskValidationService(ITaskValidationQueries queries): ITaskValidationService
{
    public async Task<ValidationErrors> ValidateCreatingTaskAsync(ITask task, CancellationToken token = default)
        => await ValidateAsync(task, token);

    public async Task<ValidationErrors> ValidateUpdatingTaskAsync(ITask task, CancellationToken token = default)
        => await ValidateAsync(task, token);

    public async Task<ValidationErrors> ValidateDeletingTaskAsync(ITask task, CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        await new TaskInTemplateValidation(queries).ValidateAsync(task, errors, token);
        return errors;
    }

    private async Task<ValidationErrors> ValidateAsync(ITask task, CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        var rules = ValidationRules();

        foreach (var rule in rules)
        {
            await rule.ValidateAsync(task, errors, token);
        }
        
        return errors;
    }
    
    private List<IValidationRule<ITask>> ValidationRules()
        => new()
        {
            new TaskTextValidation(queries),
            new TaskTypeValidation()
        };
}
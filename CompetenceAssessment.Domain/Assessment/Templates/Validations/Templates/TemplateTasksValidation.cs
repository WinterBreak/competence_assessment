using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TemplateTasksValidation: IValidationRule<ITemplate>
{
    private const int TEMPLATE_TASKS_MIN_COUNT = 5;
    public const string TEMPLATE_TASKS_MIN_COUNT_MESSAGE = "В шаблоне должно быть минимум {0} заданий";
    public const string TEMPLATE_WITH_WRONG_TASKS = "Выбраны задания, недопустимые для текущего типа шаблона";
    
    public async Task ValidateAsync(ITemplate template, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        if (template.Weights == null || !template.Weights.Any())
        {
            validationErrors.AddError(ErrorsConfg.DETAILS_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
            return;
        }

        if (template.Weights.Count < TEMPLATE_TASKS_MIN_COUNT)
        {
            var mess = string.Format(TEMPLATE_TASKS_MIN_COUNT_MESSAGE, TEMPLATE_TASKS_MIN_COUNT.ToString());
            validationErrors.AddError(ErrorsConfg.DETAILS_ERROR, mess);
        }
        
        var hasWrongTasks = template.Type == TemplateType.Survey
            ? template.Weights.Any(w => w.Task.Type != TaskType.SurveyQuestion)
            : template.Weights.Any(w => w.Task.Type == TaskType.SurveyQuestion);
        if (hasWrongTasks)
        {
            validationErrors.AddError(ErrorsConfg.DETAILS_ERROR, TEMPLATE_WITH_WRONG_TASKS);
        }
        
        var taskIds = template.Weights.Select(c => c.Task.Id).ToList();
        var hasDublicates = taskIds.Distinct().Count() != taskIds.Count;
        if (hasDublicates)
        {
            validationErrors.AddError(ErrorsConfg.REPEATING_ERROR, ErrorsConfg.REPEATING_ENTITY_ERROR);
        }
        
        // TODO а можно ли цеплять вопрос более, чем к одной компетенции?
    }
}
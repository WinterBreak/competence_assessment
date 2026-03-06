using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TaskAnswerValidation: IValidationRule<ITask>
{
    public async Task ValidateAsync(ITask task, ValidationErrors validationErrors
                            , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(validationErrors);

        if (task.Type != TaskType.TestQuestion)
        {
            return;
        }
        
        var answer = task.Answer?.Trim();
        if (string.IsNullOrEmpty(answer))
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
        }
    }
}
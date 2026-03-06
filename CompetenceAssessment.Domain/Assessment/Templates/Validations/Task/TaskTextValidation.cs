using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TaskTextValidation(ITaskValidationQueries queries): IValidationRule<ITask>
{
    public async Task ValidateAsync(ITask task, ValidationErrors validationErrors
                            , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(validationErrors);
        
        var text = task.Text?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
        }
        
        var isExist = await queries.IsTaskExistAsync(task.Text, task.Type, token);
        if (isExist)
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
        }
    }
}
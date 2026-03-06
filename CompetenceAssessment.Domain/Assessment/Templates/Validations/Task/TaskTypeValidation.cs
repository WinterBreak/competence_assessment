using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TaskTypeValidation: IValidationRule<ITask>
{
    public async Task ValidateAsync(ITask task, ValidationErrors validationErrors
        , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentNullException.ThrowIfNull(validationErrors);

        if (task.Type == TaskType.None)
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
        }
    }
}
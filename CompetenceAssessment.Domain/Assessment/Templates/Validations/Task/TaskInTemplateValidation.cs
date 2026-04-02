using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TaskInTemplateValidation(ITaskValidationQueries queries): IValidationRule<ITask>
{
    public async Task ValidateAsync(ITask task, ValidationErrors validationErrors
                            , CancellationToken token = default)
    {
        var isInTemplate = await queries.IsUsedInTemplateAsync(task.Id, token);
        if (isInTemplate)
        {
            validationErrors.AddError(ErrorsConfg.ENTITY_USING_ERROR, ErrorsConfg.USING_NAME_ERROR);
        }
    }
}
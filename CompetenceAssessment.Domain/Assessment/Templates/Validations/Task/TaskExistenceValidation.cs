using CompetenceAssessment.Core.Confings;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public class TaskExistenceValidation: IValidationRule<ITask>
{
    private readonly ITaskValidationQueries _queries;

    public TaskExistenceValidation(ITaskValidationQueries queries)
    {
        _queries = queries;
    }
    
    public async Task ValidateAsync(ITask task, ValidationErrors validationErrors
                                  , CancellationToken token = default)
    {
        var isExist = await _queries.IsTaskExistAsync(task.Id, token);
        if (!isExist)
        {
            validationErrors.AddError(ErrorsConfg.NON_EXISTENT_ENTITY, ErrorsConfg.NON_EXISTENT_ENTITY_ERROR);
        }
    }
}
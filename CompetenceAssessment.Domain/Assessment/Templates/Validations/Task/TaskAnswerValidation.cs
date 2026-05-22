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

        if (task.Type != TaskType.TestQuestion && task.Answers != null && task.Answers.Count != 0)
        {
            validationErrors.AddError(ErrorsConfg.TASK_ERROR, ErrorsConfg.ANSWER_FOR_WRONG_TASK_TYPE);
            return;
        }

        var hasNoAnswers = task.Answers is null || !task.Answers.Any();
        if (hasNoAnswers)
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
            return;
        }
        
        var hasCorrectAnswer = task.Answers.Any(a => a.IsCorrect);
        if (!hasCorrectAnswer)
        {
            validationErrors.AddError(ErrorsConfg.NAME_ERROR, ErrorsConfg.EMPTY_FIELD_ERROR);
            return;
        }
    }
}
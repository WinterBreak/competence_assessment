using CompetenceAssessment.Application.Assessment;

namespace CompetenceAssessment.Domain.Assessment;

public class TaskValidationServiceFactory(ITaskValidationQueries queries): ITaskValidationServiceFactory
{
    public ITaskValidationService Create(TaskType taskType)
    {
        return taskType switch
        {
            TaskType.TestQuestion => new TestTaskValidationService(queries),
            TaskType.SurveyQuestion => new SurveyTaskValidationService(queries),
            TaskType.OpenQuestion => new SurveyTaskValidationService(queries),
            TaskType.None => new SurveyTaskValidationService(queries),
            _ => throw new ArgumentException($"Неизвестный тип задания: {taskType}")
        };
    }
}
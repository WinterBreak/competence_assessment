namespace CompetenceAssessment.Domain.Assessment;

public interface ITaskValidationServiceFactory
{
    ITaskValidationService Create(TaskType taskType);
}
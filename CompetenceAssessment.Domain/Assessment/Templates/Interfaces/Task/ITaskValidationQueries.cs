namespace CompetenceAssessment.Domain.Assessment;

public interface ITaskValidationQueries
{
    Task<bool> IsTaskExistAsync(string text, TaskType type, CancellationToken token = default);
    
    Task<bool> IsUsedInTemplateAsync(int taskId, CancellationToken token = default);
}
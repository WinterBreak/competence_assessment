namespace CompetenceAssessment.Domain.Assessment;

public interface ITaskValidationQueries
{
    Task<bool> IsTaskExistAsync(string text, TaskType type, int? id = null, CancellationToken token = default);
    
    Task<bool> IsUsedInTemplateAsync(int id, CancellationToken token = default);
    
    Task<bool> IsTaskExistAsync(int id, CancellationToken token = default);
}
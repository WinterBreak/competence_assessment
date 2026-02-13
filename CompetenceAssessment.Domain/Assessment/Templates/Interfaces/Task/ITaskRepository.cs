namespace CompetenceAssessment.Domain.Assessment;

public interface ITaskRepository
{
    Task<ITask?> GetTaskAsync(TaskQuery query, CancellationToken cancellationToken = default);
    
    Task<List<ITask>> GetTasksAsync(TaskQuery query, CancellationToken cancellationToken = default);
    
    Task AddTaskAsync(ITask task, CancellationToken cancellationToken = default);
    
    Task UpdateTaskAsync(ITask task, CancellationToken cancellationToken = default);
    
    Task RemoveTaskAsync(ITask task, CancellationToken cancellationToken = default);
    
    Task SaveAllChangesAsync(CancellationToken cancellationToken = default);
}
namespace CompetenceAssessment.Domain.Assessment;

public interface ITaskRepository
{
    Task<ITask?> GetTaskAsync(TaskQuery query, CancellationToken token = default);
    
    Task<List<ITask>> GetTasksAsync(TaskQuery query, CancellationToken token = default);
    
    Task AddTaskAsync(ITask task, CancellationToken token = default);
    
    Task UpdateTaskAsync(ITask task, CancellationToken token = default);
    
    Task RemoveTaskAsync(ITask task, CancellationToken token = default);
    
    Task SaveAllChangesAsync(CancellationToken token = default);
}
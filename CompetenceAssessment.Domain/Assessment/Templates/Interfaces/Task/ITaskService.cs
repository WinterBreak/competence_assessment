using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ITaskService
{
    Task<ITask?> GetTaskAsync(TaskQuery query, CancellationToken cancellationToken = default);
    
    Task<List<ITask>> GetTasksAsync(TaskQuery query, CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> UpdateTaskAsync(UpdateTaskCommand command, CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> DeleteTaskAsync(DeleteTaskCommand command, CancellationToken cancellationToken = default);
}
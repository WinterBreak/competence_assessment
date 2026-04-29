using CompetenceAssessment.Core.Models;
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ITaskService
{
    Task<ITask?> GetTaskAsync(TaskQuery query, CancellationToken token = default);
    
    Task<List<ITask>> GetTasksAsync(TaskQuery query, CancellationToken token = default);
    
    Task<PaginatedResponse<ITask>> GetPaginatedTasksAsync(TaskQuery query, CancellationToken token = default);
    
    Task<ValidationErrors> CreateTaskAsync(CreateTaskCommand command, CancellationToken token = default);
    
    Task<ValidationErrors> UpdateTaskAsync(UpdateTaskCommand command, CancellationToken token = default);
    
    Task<ValidationErrors> DeleteTaskAsync(DeleteTaskCommand command, CancellationToken token = default);
}
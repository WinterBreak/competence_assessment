using CompetenceAssessment.Core.Models;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment.Templates;

public class TaskService: ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly ITaskValidationQueries _queries;
    private readonly ITaskValidationServiceFactory _validationFactory;
    
    public TaskService(ITaskRepository repository
        , ITaskValidationQueries queries)
    {
        _repository = repository;
        _queries = queries;
        _validationFactory = new TaskValidationServiceFactory(_queries);
    }
    
    public async Task<ITask?> GetTaskAsync(TaskQuery query, CancellationToken token = default)
        => await _repository.GetTaskAsync(query, token);

    public async Task<List<ITask>> GetTasksAsync(TaskQuery query, CancellationToken token = default)
        => await _repository.GetTasksAsync(query, token);
    
    public async Task<PaginatedResponse<ITask>> GetPaginatedTasksAsync(TaskQuery query, CancellationToken token = default)
        => await _repository.GetPaginatedTasksAsync(query, token);

    public async Task<ValidationErrors> CreateTaskAsync(CreateTaskCommand command
                                                      , CancellationToken token = default)
    {
        var newTask = command.Create();
        var validationService = _validationFactory.Create(newTask.Type);
        var errors = await validationService.ValidateCreatingTaskAsync(newTask, token);

        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.AddTaskAsync(newTask, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }

    public async Task<ValidationErrors> UpdateTaskAsync(UpdateTaskCommand command
                                                      , CancellationToken token = default)
    {
        var validationService = _validationFactory.Create(command.Type);
        var errors = await validationService.ValidateExistenceAsync(command.Id, token);
        if (errors.HasErrors)
        {
            return errors;
        }
            
        var query = new TaskQuery(id: command.Id);
        var updatingTask = await _repository.GetTaskAsync(query, token);
        
        command.Update(updatingTask);
        
        errors = await validationService.ValidateUpdatingTaskAsync(updatingTask, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.UpdateTaskAsync(updatingTask, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }

    public async Task<ValidationErrors> DeleteTaskAsync(DeleteTaskCommand command
                                                      , CancellationToken token = default)
    {
        var validationService = _validationFactory.Create(TaskType.None);
        var errors = await validationService.ValidateDeletingTaskAsync(command.Id, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.RemoveTaskAsync(command.Id, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }
}
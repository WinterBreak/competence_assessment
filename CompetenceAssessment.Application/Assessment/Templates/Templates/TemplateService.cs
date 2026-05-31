using CompetenceAssessment.Core.Models;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class TemplateService: ITemplateService
{
    private readonly ITemplateRepository _repository;
    private readonly ITaskRepository _taskRepository;
    private readonly ITemplateValidationService _validationService;

    public TemplateService(ITemplateRepository repository
                         , ITaskRepository taskRepository
                         , ITemplateValidationService validationService)
    {
        _repository = repository;
        _taskRepository = taskRepository;
        _validationService = validationService;
    }
    
    public async Task<ITemplate?> GetTemplateAsync(TemplateQuery query, CancellationToken token = default)
        => await _repository.GetTemplateAsync(query, token);

    public async Task<List<ITemplate>> GetTemplatesAsync(TemplateQuery query, CancellationToken token = default)
        => await _repository.GetTemplatesAsync(query, token);

    public async Task<PaginatedResponse<ITemplate>> GetPaginatedTemplatesAsync(TemplateQuery query
        , CancellationToken token = default)
        => await _repository.GetPaginatedTemplatesAsync(query, token);

    public async Task<ValidationErrors> CreateTemplateAsync(CreateTemplateCommand command
                                                          , CancellationToken token = default)
    {
        var tasks = await GetTasks(command.Weights.Keys.ToList(), token);
        var newTemplate = command.Create(tasks);
        
        var errors = await _validationService.ValidateCreatingTemplateAsync(newTemplate, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.AddTemplateAsync(newTemplate, token);
        await _repository.SaveAllChanges(token);
        return errors;
    }

    public async Task<ValidationErrors> UpdateTemplateAsync(UpdateTemplateCommand command
                                                          , CancellationToken token = default)
    {
        var errors = await _validationService.ValidateExistenceAsync(command.Id, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        var query = new TemplateQuery(id: command.Id);
        var updatingTemplate = await _repository.GetTemplateAsync(query, token);
        
        var tasks = await GetTasks(command.Weights.Keys.ToList(), token);
        command.Update(updatingTemplate, tasks);
        
        errors = await _validationService.ValidateUpdatingTemplateAsync(updatingTemplate, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.UpdateTemplateAsync(updatingTemplate, token);
        await _repository.SaveAllChanges(token);
        return errors;
    }

    public async Task<ValidationErrors> DeleteTemplateAsync(DeleteTemplateCommand command
                                                          , CancellationToken token = default)
    {
        var errors = await _validationService.ValidateDeletingTemplateAsync(command.Id, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.RemoveTemplateAsync(command.Id, token);
        await _repository.SaveAllChanges(token);
        return errors;
    }

    private async Task<List<ITask>> GetTasks(List<int> taskIds
        , CancellationToken token = default)
    {
        var taskQuery = new TaskQuery(ids: taskIds);
        return await _taskRepository.GetTasksAsync(taskQuery, token);
    }
}
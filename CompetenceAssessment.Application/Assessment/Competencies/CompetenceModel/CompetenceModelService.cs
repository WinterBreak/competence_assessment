using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class CompetenceModelService: ICompetenceModelService
{
    private readonly ICompetenceModelRepository _repository;
    private readonly ICompetenceModelValidationService _validationService;

    public CompetenceModelService(ICompetenceModelRepository repository
        , ICompetenceModelValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
    }
    
    public async Task<CompetenceModel?> GetCompetenceModelAsync(CompetenceModelQuery query
        , CancellationToken token = default)
        => await _repository.GetCompetenceModelAsync(query, token);

    public async Task<List<CompetenceModel>> GetCompetenceModelsAsync(CompetenceModelQuery query
        , CancellationToken token = default)
        => await _repository.GetCompetenceModelsAsync(query, token);

    public async Task<ValidationErrors> CreateCompetenceModelAsync(CreateCompetenceModelCommand command
        , CancellationToken token = default)
    {
        var newModel = command.Create();
        
        var errors = await _validationService.ValidateCreatingCompetenceModelAsync(newModel, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.AddCompetenceModelAsync(newModel, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }

    public async Task<ValidationErrors> UpdateCompetenceModelAsync(UpdateCompetenceModelCommand command
        , CancellationToken token = default)
    {
        var errors = await _validationService.ValidateExistence(command.Id, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        var query = new CompetenceModelQuery(id: command.Id);
        var updatingModel = await _repository.GetCompetenceModelAsync(query, token);
        
        command.Update(updatingModel);
        
        errors = await _validationService.ValidateUpdatingCompetenceModelAsync(updatingModel, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.UpdateCompetenceModelAsync(updatingModel, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }

    public async Task<ValidationErrors> DeleteCompetenceModelAsync(DeleteCompetenceModelCommand command
        , CancellationToken token = default)
    {
        var errors = await _validationService.ValidateDeletingCompetenceModelAsync(command.Id, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.RemoveCompetenceModelAsync(command.Id, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }
}
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class CompetenceService: ICompetenceService
{
    private readonly ICompetenceRepository _repository;
    private readonly ICompetenceValidationService _validationService;

    public CompetenceService(ICompetenceRepository repository
        , ICompetenceValidationService validationService)
    {
        _repository = repository;
        _validationService = validationService;
    }
    
    public async Task<Competence?> GetCompetenceAsync(CompetenceQuery query
        , CancellationToken token = default)
        => await _repository.GetCompetenceAsync(query, token);

    public async Task<List<Competence>> GetCompetenciesAsync(CompetenceQuery query
        , CancellationToken token = default)
        => await _repository.GetCompetenciesAsync(query, token);

    public async Task<ValidationErrors> CreateCompetenceAsync(CreateCompetenceCommand command
        , CancellationToken token = default)
    {
        var newCompetence = command.Create();
        var errors = await _validationService
            .ValidateCreatingCompetenceAsync(newCompetence, token);

        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.AddCompetenceAsync(newCompetence, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }

    public async Task<ValidationErrors> UpdateCompetenceAsync(UpdateCompetenceCommand command
        , CancellationToken token = default)
    {
        var query = new CompetenceQuery(id: command.Id);
        var updatingCompetence = await _repository.GetCompetenceAsync(query, token);
        ArgumentNullException.ThrowIfNull(updatingCompetence);
        
        command.Update(updatingCompetence);
        
        var errors = await _validationService
            .ValidateUpdatingCompetenceAsync(updatingCompetence, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.UpdateCompetenceAsync(updatingCompetence, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }

    public async Task<ValidationErrors> DeleteCompetenceAsync(DeleteCompetenceCommand command
        , CancellationToken token = default)
    {
        var query = new CompetenceQuery(id: command.Id);
        var deletingCompetence = await _repository.GetCompetenceAsync(query, token);
        ArgumentNullException.ThrowIfNull(deletingCompetence);
        
        var errors = await _validationService
            .ValidateDeletingCompetenceAsync(deletingCompetence, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.RemoveCompetenceAsync(command.Id, token);
        await _repository.SaveAllChangesAsync(token);
        return errors;
    }
}
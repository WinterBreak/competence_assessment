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
        , CancellationToken cancellationToken = default)
        => await _repository.GetCompetenceAsync(query, cancellationToken);

    public async Task<List<Competence>> GetCompetenciesAsync(CompetenceQuery query
        , CancellationToken cancellationToken = default)
        => await _repository.GetCompetenciesAsync(query, cancellationToken);

    public async Task<ValidationErrors> CreateCompetenceAsync(CreateCompetenceCommand command
        , CancellationToken cancellationToken = default)
    {
        var newCompetence = command.Create();
        var errors = await _validationService
            .ValidateCreatingCompetenceAsync(newCompetence, cancellationToken);

        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.AddCompetenceAsync(newCompetence, cancellationToken);
        await _repository.SaveAllChangesAsync(cancellationToken);
        return errors;
    }

    public async Task<ValidationErrors> UpdateCompetenceAsync(UpdateCompetenceCommand command
        , CancellationToken cancellationToken = default)
    {
        var query = new CompetenceQuery(id: command.Id);
        var updatingCompetence = await _repository.GetCompetenceAsync(query, cancellationToken);
        ArgumentNullException.ThrowIfNull(updatingCompetence);
        
        command.Update(updatingCompetence);
        
        var errors = await _validationService
            .ValidateUpdatingCompetenceAsync(updatingCompetence, cancellationToken);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.UpdateCompetenceAsync(updatingCompetence, cancellationToken);
        await _repository.SaveAllChangesAsync(cancellationToken);
        return errors;
    }

    public async Task<ValidationErrors> DeleteCompetenceAsync(DeleteCompetenceCommand command
        , CancellationToken cancellationToken = default)
    {
        var query = new CompetenceQuery(id: command.Id);
        var deletingCompetence = await _repository.GetCompetenceAsync(query, cancellationToken);
        ArgumentNullException.ThrowIfNull(deletingCompetence);
        
        var errors = await _validationService
            .ValidateDeletingCompetenceAsync(deletingCompetence, cancellationToken);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        await _repository.RemoveCompetenceAsync(command.Id, cancellationToken);
        await _repository.SaveAllChangesAsync(cancellationToken);
        return errors;
    }
}
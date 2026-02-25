using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.Competencies.Validations;

namespace CompetenceAssessment.Application.Assessment;

public class CompetenceValidationService: ICompetenceValidationService
{
    private readonly ICompetenceRepository _repository;

    public CompetenceValidationService(ICompetenceRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ValidationErrors> ValidateCreatingCompetenceAsync(Competence competence
        , CancellationToken cancellationToken = default)
        => await ValidateCompetenceFieldsAsync(competence, cancellationToken);

    public async Task<ValidationErrors> ValidateUpdatingCompetenceAsync(Competence competence
        , CancellationToken cancellationToken = default)
        => await ValidateCompetenceFieldsAsync(competence, cancellationToken);

    public Task<ValidationErrors> ValidateDeletingCompetenceAsync(Competence competence
        , CancellationToken cancellationToken = default)
    {
        // TODO rule, проверяющий, что компетенция не относится ни к одной модели
        throw new NotImplementedException();
    }

    private async Task<ValidationErrors> ValidateCompetenceFieldsAsync(Competence competence
        , CancellationToken cancellationToken = default)
    {
        var errors = new ValidationErrors();
        var rules = GetFieldValidationRules();

        foreach (var rule in rules)
        {
            await rule.ValidateAsync(competence, errors, cancellationToken);
        }
        
        return errors;
    }
    
    private List<IValidationRule<Competence>> GetFieldValidationRules()
        => new()
        {
            new CompetenceDescriptionValidation(),
            new CompetenceNameValidation(_repository), 
        };
}
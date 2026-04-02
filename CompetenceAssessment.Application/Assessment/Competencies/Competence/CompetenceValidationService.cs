using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.Competencies.Validations;
using CompetenceAssessment.Domain.Assessment.Competencies.Validations.Competencies;

namespace CompetenceAssessment.Application.Assessment;

public class CompetenceValidationService: ICompetenceValidationService
{
    private readonly ICompetenceValidationQueries _queries;

    public CompetenceValidationService(ICompetenceValidationQueries queries)
    {
        _queries = queries;
    }
    
    public async Task<ValidationErrors> ValidateCreatingCompetenceAsync(Competence competence
        , CancellationToken token = default)
        => await ValidateAsync(competence, token);

    public async Task<ValidationErrors> ValidateUpdatingCompetenceAsync(Competence competence
        , CancellationToken token = default)
    {
        return await ValidateAsync(competence, token);
    }
         

    public async Task<ValidationErrors> ValidateDeletingCompetenceAsync(int id, CancellationToken token = default)
    {
        var errors = await ValidateExistenceAsync(id, token);
        if (errors.HasErrors)
        {
            return errors;
        }
        
        var competence = new Competence {Id = id};
        await new CompetenceInModelValidation(_queries).ValidateAsync(competence, errors, token);
        return errors;
    }
    
    public async Task<ValidationErrors> ValidateExistenceAsync(int id, CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        var competence = new Competence {Id = id};
        await new CompetenceExistsValidation(_queries).ValidateAsync(competence, errors, token);
        return errors;
    }

    private async Task<ValidationErrors> ValidateAsync(Competence competence
        , CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        var rules = GetFieldValidationRules();

        foreach (var rule in rules)
        {
            await rule.ValidateAsync(competence, errors, token);
        }
        
        return errors;
    }
    
    private List<IValidationRule<Competence>> GetFieldValidationRules()
        => new()
        {
            new CompetenceDescriptionValidation(),
            new CompetenceNameValidation(_queries), 
        };
}
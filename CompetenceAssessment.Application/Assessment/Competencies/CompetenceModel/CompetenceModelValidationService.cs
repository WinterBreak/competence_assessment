using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class CompetenceModelValidationService: ICompetenceModelValidationService
{
    private readonly ICompetenceModelValidationQueries _queries;

    public CompetenceModelValidationService(ICompetenceModelValidationQueries queries)
    {
        _queries = queries;
    }

    public async Task<ValidationErrors> ValidateCreatingCompetenceModelAsync(CompetenceModel model,
        CancellationToken token = default)
        => await ValidateAsync(model, token);

    public async Task<ValidationErrors> ValidateUpdatingCompetenceModelAsync(CompetenceModel model,
        CancellationToken token = default)
        => await ValidateAsync(model, token);
    
    public async Task<ValidationErrors> ValidateDeletingCompetenceModelAsync(CompetenceModel model
        , CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        await new CompetenceModelInTemplateValidation(_queries).ValidateAsync(model, errors, token);
        return errors;
    }

    public async Task<ValidationErrors> ValidateExistence(int id, CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel{Id = id};
        await new CompetenceModelExistValidation(_queries).ValidateAsync(model, errors, token);
        return errors;
    }

    private async Task<ValidationErrors> ValidateAsync(CompetenceModel model
        , CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        var rules = GetFieldValidationRules();

        foreach (var rule in rules)
        {
            await rule.ValidateAsync(model, errors, token);
        }
        
        return errors;
    }
    
    private List<IValidationRule<CompetenceModel>> GetFieldValidationRules()
        => new()
        {
            new CompetenceModelNameValidation(_queries),
            new CompetenceModelDescriptionValidation(),
            new CompetenceModelWeightValidation()
        };
}
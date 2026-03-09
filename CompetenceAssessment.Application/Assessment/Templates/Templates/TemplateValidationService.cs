using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class TemplateValidationService(ITemplateValidationQueries queries): ITemplateValidationService
{
    public async Task<ValidationErrors> ValidateCreatingTemplateAsync(ITemplate template
                                                                    , CancellationToken token = default)
        => await ValidateAsync(template, token);

    public async Task<ValidationErrors> ValidateUpdatingTemplateAsync(ITemplate template
                                                                    , CancellationToken token = default)
        => await ValidateAsync(template, token);

    public async Task<ValidationErrors> ValidateDeletingTemplateAsync(ITemplate template
                                                                    , CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        await new TemplateInAssessmentValidation(queries).ValidateAsync(template, errors, token);
        return errors;
    }
    
    private async Task<ValidationErrors> ValidateAsync(ITemplate template
                                                     , CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        var rules = GetFieldValidationRules();

        foreach (var rule in rules)
        {
            await rule.ValidateAsync(template, errors, token);
        }
        
        return errors;
    }
    
    private List<IValidationRule<ITemplate>> GetFieldValidationRules()
        => new()
        {
            new TemplateNameValidation(queries),
            new TemplateTypeValidation(),
            new TemplateScaleValidation(),
            new TemplateCompetenceModelValidation(),
            new TemplateTasksValidation()
        };
}
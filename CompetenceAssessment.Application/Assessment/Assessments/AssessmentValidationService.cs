using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class AssessmentValidationService: IAssessmentValidationService
{
    private readonly IAssessmentValidationQueries _queries;
    
    public async Task<ValidationErrors> ValidateCreatingAssessmentAsync(
        Domain.Assessment.Assessment assessment, CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        var rules = GetValidationRulesForCreating();

        foreach (var rule in rules)
        {
            await rule.ValidateAsync(assessment, errors, token);
        }
        
        return errors;
    }

    public async Task<ValidationErrors> ValidateUpdatingAssessmentAsync(
        Domain.Assessment.Assessment assessment, CancellationToken token = default)
    {
        var errors = new ValidationErrors();
        await new AssessmentResultValidation().ValidateAsync(assessment, errors, token);
        return errors;
    }
    
    private List<IValidationRule<Domain.Assessment.Assessment>> GetValidationRulesForCreating() 
        => new()
        {
            new AssessmentTemplateValidation(),
            new AssessmentCandidateValidation(),
            new AssessmentInspectorsValidation(),
            new AssessmentStartedValidation(_queries)
       };
}
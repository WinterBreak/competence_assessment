using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ITemplateValidationService
{
    Task<ValidationErrors> ValidateCreatingTemplateAsync(ITemplate template, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateUpdatingTemplateAsync(ITemplate template, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateDeletingTemplateAsync(ITemplate template, CancellationToken token = default);
}
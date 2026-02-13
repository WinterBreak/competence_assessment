using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ITemplateValidationService
{
    Task<ValidationErrors> ValidateCreatingTemplate(ITemplate template, CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> ValidateUpdatingTemplate(ITemplate template, CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> ValidateDeletingTemplate(ITemplate template, CancellationToken cancellationToken = default);
}
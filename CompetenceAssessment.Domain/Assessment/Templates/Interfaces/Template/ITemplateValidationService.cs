using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ITemplateValidationService
{
    Task<ValidationErrors> ValidateCreatingTemplateAsync(ITemplate template, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateUpdatingTemplateAsync(ITemplate template, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateDeletingTemplateAsync(int id, CancellationToken token = default);

    Task<ValidationErrors> ValidateExistenceAsync(int id, CancellationToken token = default);
}
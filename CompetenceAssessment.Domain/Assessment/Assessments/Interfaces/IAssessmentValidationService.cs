using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentValidationService
{
    Task<ValidationErrors> ValidateCreatingAssessmentAsync(Assessment assessment
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> ValidateUpdatingAssessmentAsync(Assessment assessment
        , CancellationToken cancellationToken = default);
}
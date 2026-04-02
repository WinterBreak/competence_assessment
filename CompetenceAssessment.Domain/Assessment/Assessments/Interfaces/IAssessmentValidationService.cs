using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentValidationService
{
    Task<ValidationErrors> ValidateCreatingAssessmentAsync(Assessment assessment, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateUpdatingAssessmentAsync(Assessment assessment, CancellationToken token = default);
    
    Task<ValidationErrors> ValidateExistenceAsync(int id, CancellationToken token = default);
}
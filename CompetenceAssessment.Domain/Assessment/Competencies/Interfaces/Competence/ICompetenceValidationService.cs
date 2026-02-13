using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceValidationService
{
    Task<ValidationErrors> ValidateCreatingCompetenceAsync(Competence competence
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> ValidateUpdatingCompetenceAsync(Competence competence
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> ValidateDeletingCompetenceAsync(Competence competence
        , CancellationToken cancellationToken = default);
}
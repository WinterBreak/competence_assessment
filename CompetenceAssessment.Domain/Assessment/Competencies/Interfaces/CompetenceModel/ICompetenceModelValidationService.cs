using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceModelValidationService
{
    Task<ValidationErrors> ValidateCreatingCompetenceModelAsync(CompetenceModel competenceModel
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> ValidateUpdatingCompetenceModelAsync(CompetenceModel competenceModel
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> ValidateDeletingCompetenceModelAsync(CompetenceModel competenceModel
        , CancellationToken cancellationToken = default);
}
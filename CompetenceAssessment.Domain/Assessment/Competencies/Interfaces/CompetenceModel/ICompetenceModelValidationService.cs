using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceModelValidationService
{
    Task<ValidationErrors> ValidateCreatingCompetenceModelAsync(CompetenceModel model
        , CancellationToken token = default);
    
    Task<ValidationErrors> ValidateUpdatingCompetenceModelAsync(CompetenceModel model
        , CancellationToken token = default);
    
    Task<ValidationErrors> ValidateDeletingCompetenceModelAsync(CompetenceModel model
        , CancellationToken token = default);
}
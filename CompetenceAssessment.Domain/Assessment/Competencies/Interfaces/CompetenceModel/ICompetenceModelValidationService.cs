using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceModelValidationService
{
    Task<ValidationErrors> ValidateCreatingCompetenceModelAsync(CompetenceModel model
        , CancellationToken token = default);
    
    Task<ValidationErrors> ValidateUpdatingCompetenceModelAsync(CompetenceModel model
        , CancellationToken token = default);
    
    Task<ValidationErrors> ValidateDeletingCompetenceModelAsync(int id, CancellationToken token = default);

    Task<ValidationErrors> ValidateExistence(int id, CancellationToken token = default);
}
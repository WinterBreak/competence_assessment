using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceModelService
{
    Task<CompetenceModel?> GetCompetenceModelAsync(CompetenceModelQuery query
        , CancellationToken cancellationToken = default);
    
    Task<List<CompetenceModel>> GetCompetenceModelsAsync(CompetenceModelQuery query
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> CreateCompetenceModelAsync(CreateCompetenceModelCommand command
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> UpdateCompetenceModelAsync(UpdateCompetenceModelCommand command
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> DeleteCompetenceModelAsync(DeleteCompetenceModelCommand command
        , CancellationToken cancellationToken = default);
}
using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceModelService
{
    Task<CompetenceModel?> GetCompetenceModelAsync(CompetenceModelQuery query
        , CancellationToken token = default);
    
    Task<List<CompetenceModel>> GetCompetenceModelsAsync(CompetenceModelQuery query
        , CancellationToken token = default);
    
    Task<ValidationErrors> CreateCompetenceModelAsync(CreateCompetenceModelCommand command
        , CancellationToken token = default);
    
    Task<ValidationErrors> UpdateCompetenceModelAsync(UpdateCompetenceModelCommand command
        , CancellationToken token = default);
    
    Task<ValidationErrors> DeleteCompetenceModelAsync(DeleteCompetenceModelCommand command
        , CancellationToken token = default);
}
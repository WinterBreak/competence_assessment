using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceService
{
    Task<Competence?> GetCompetenceAsync(CompetenceQuery query, CancellationToken token = default);
    
    Task<List<Competence>> GetCompetenciesAsync(CompetenceQuery query, CancellationToken token = default);
    
    Task<ValidationErrors> CreateCompetenceAsync(CreateCompetenceCommand command
        , CancellationToken token = default);
    
    Task<ValidationErrors> UpdateCompetenceAsync(UpdateCompetenceCommand command
        , CancellationToken token = default);

    Task<ValidationErrors> DeleteCompetenceAsync(DeleteCompetenceCommand command
        , CancellationToken token = default);
}